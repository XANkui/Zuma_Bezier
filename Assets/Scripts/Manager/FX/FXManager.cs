using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public static FXManager Instance;
    public GameObject mDestroyFXPrefab;

    private ObjectPool<GameObject> mDestroyFXPool;
    private void Awake()
    {
        Instance = this;
        mDestroyFXPool = new ObjectPool<GameObject>(InstanceObject, 10);
    }

    private GameObject InstanceObject() {
        GameObject fx = Instantiate(mDestroyFXPrefab, transform);
        fx.SetActive(false);
        return fx;
    }

    public void ShowDestroyFX(Vector3 pos) {
        GameObject fx = mDestroyFXPool.GetObject();
        fx.SetActive(true);
        fx.transform.localPosition = pos;

        // 延时 0.5f 执行回收
        ScheduleOnce.Start(this, ()=> {
            fx.SetActive(false);
            mDestroyFXPool.AddObject(fx);
        },0.5f);
    }
}
