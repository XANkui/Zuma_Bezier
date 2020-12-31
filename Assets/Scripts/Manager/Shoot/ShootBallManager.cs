using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBallManager : MonoBehaviour
{
    public static ShootBallManager Instance;

    public GameObject mBallPrefab;

    private ObjectPool<ShootBall> mPool;
    /// <summary>
    /// 当前发射球集合
    /// </summary>
    public List<ShootBall> mShootBallLst = new List<ShootBall>();
    private void Awake()
    {
        Instance = this;
        mPool = new ObjectPool<ShootBall>(InstanceObject, 3);
    }

    private ShootBall InstanceObject() {
        GameObject ball = Instantiate(mBallPrefab,transform);
        ball.SetActive(false);
        ShootBall shootBall = ball.AddComponent<ShootBall>();

        return shootBall;
    }

    private void Update()
    {
        for (int i = mShootBallLst.Count- 1; i >=0 ; i--)
        {
            mShootBallLst[i].Move();
            if (mShootBallLst[i].IsOutOfBounds())
            {
                Recovery(mShootBallLst[i]);
                mShootBallLst.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 回收发射后超出屏幕的子弹
    /// </summary>
    public void Recovery(ShootBall shootBall) {
        shootBall.gameObject.SetActive(false);
        mPool.AddObject(shootBall);
    }

    public void Shoot(BallType ballType,Sprite sp, Transform trans) {
        ShootBall shootBall = mPool.GetObject();
        shootBall.Init(ballType, sp, trans);
        mShootBallLst.Add(shootBall);
    }
}
