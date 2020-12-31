using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class BezierPathController : MonoBehaviour
{
    public bool mIsGizmos = true;
    public GameObject mBallPrefab;
    public float mZumaBallDistance = 0.31f;
    public float mGizmosSphereRadiu = 0.15f;
    public int mSegmentsPerCurve = 10;
    public List<GameObject> mControlPointLst = new List<GameObject>();
    public List<Vector3> mBallPointLst = new List<Vector3>();


    private void Awake()
    {
        foreach (var item in mBallPointLst)
        {
            GameObject ball = Instantiate(mBallPrefab,GameObject.Find("Map").transform);
            ball.transform.position = item;
        }
    }

    private void OnDrawGizmos()
    {
        if (mIsGizmos==false)
        {
            foreach (Transform item in this.transform) {
                item.gameObject.SetActive(false);
            }
            return;
        }

        mControlPointLst.Clear();

        foreach (Transform item in this.transform)
        {
            //Debug.Log(GetType()+ "/OnDrawGizmos()/:");
            item.gameObject.SetActive(true);
            mControlPointLst.Add(item.gameObject);
        }

        List<Vector3> controlPointPos = mControlPointLst.Select(point => point.transform.position).ToList();

        var points = GetDrawingPoints(controlPointPos, mSegmentsPerCurve);

        // 根据祖玛游戏中球的大小找到贝塞尔曲线点中合适的点
        Vector3 startPos = points[0];
        mBallPointLst.Clear();
        mBallPointLst.Add(startPos);
        for (int i = 1; i < points.Count; i++)
        {
            if (Vector3.Distance(startPos,points[i])>= mZumaBallDistance)
            {
                startPos = points[i];
                mBallPointLst.Add(startPos);
            }
        }

        Gizmos.color = Color.blue;
        foreach (var item in mBallPointLst)
        {
            Gizmos.DrawSphere(item, mGizmosSphereRadiu);
        }
        // 贝塞尔曲线点连线
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }

        // 贝塞尔控制点连线
        Gizmos.color = Color.red;
        for (int i = 0; i < controlPointPos.Count-1; i++)
        {
            Gizmos.DrawLine(controlPointPos[i],controlPointPos[i+1]);
        }
    }

    public List<Vector3> GetDrawingPoints(List<Vector3> controlPoints, int segmentsPerCurve) {
        List<Vector3> pointsCurve = new List<Vector3>();
        
        for (int i = 0; i < controlPoints.Count-3; i+= 3)
        {
            var p0 = controlPoints[i];
            var p1 = controlPoints[i+1];
            var p2 = controlPoints[i+2];
            var p3 = controlPoints[i+3];

            for (int j = 0; j <= segmentsPerCurve; j++)
            {
                var t = j / (float)segmentsPerCurve;
                pointsCurve.Add(CalculateBezierPoint(t,p0,p1,p2,p3));
            }
        }

        return pointsCurve;
    }

    public Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        
        
        var x = 1 - t;
        var xx = x * x;
        var xxx = xx * x;
        var tt = t * t;
        var ttt = tt * t;

        // 3次方的贝尔曲线
        return p0 * xxx + 3 * p1 * t * xx + 3 * p2 * tt * x + p3 * ttt;
    }

    public void CreateMapAsset() {
        string assetPath = "Assets/Resources/Map/map.asset";
        MapConfig mapConfig = new MapConfig();

        foreach (var item in mBallPointLst)
        {
            mapConfig.pathPointLst.Add(item);
        }

        AssetDatabase.CreateAsset(mapConfig, assetPath);
        AssetDatabase.SaveAssets();
    }
}

[CustomEditor(typeof(BezierPathController))]
public class BezierEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("生成地图文件"))
        {
            (target as BezierPathController).CreateMapAsset();
            Debug.Log(GetType()+ "/OnInspectorGUI()/:生成地图文件成功");
        }
    }
}
