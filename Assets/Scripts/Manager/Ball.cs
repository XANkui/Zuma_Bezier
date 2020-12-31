using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float mProgress = 0;
    /// <summary>
    /// 是否销毁标记
    /// </summary>
    public bool mIsDestory = false;
    private GameManager mGameManager;

    public BallType mBallType;
    /// <summary>
    /// 回退到哪个球后面
    /// </summary>
    public Ball mFallbackBall;
    private SpriteRenderer mSpriteRenderer;

    /// <summary>
    /// 生成的时候调用
    /// </summary>
    public void SpawnInit() {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 每次显示的时候调用
    /// </summary>
    /// <param name="gameManager"></param>
    /// <param name="ballType"></param>
    /// <returns></returns>
    public Ball Init(GameManager gameManager, BallType ballType) {
        mGameManager = gameManager;
        mBallType = ballType;
        mSpriteRenderer.sprite = mGameManager.GetBallTypeSprite(ballType);
        this.transform.localPosition = Vector3.one * 1000;
        this.gameObject.SetActive(true);
        Next = null;
        Pre = null;
        mProgress = 0;
        mIsDestory = false;
        return this;
    }

    private void Update()
    {
        if (mProgress >= 0)
            transform.localPosition = mGameManager.mMapConfig.GetPosition(mProgress);
    }

    public bool IsExitStartHole { 
        get {
            return mProgress >= 1;
        } 
    }

    /// <summary>
    /// 是否到达终点洞口
    /// </summary>
    public bool IsArriveFailHole {
        get {
            //Debug.Log(GetType()+ "/IsArriveFailHole()/ mProgress :" + mProgress);
            //Debug.Log(GetType()+ "/IsArriveFailHole()/ mGameManager.mMapConfig.EndPoint :" + mGameManager.mMapConfig.EndPoint);
            return mProgress >= mGameManager.mMapConfig.EndPoint;
        }
    }

    public Ball Next { get; set; }
    public Ball Pre { get; set; }

    /// <summary>
    /// 获取该队列球的尾球
    /// </summary>
    public Ball Tail {
        get
        {
            Ball ball = this;
            do
            {
                //Debug.Log(GetType() + "/Tail()/");
                

                if (ball.Next == null)
                {
                    return ball;
                }
                ball = ball.Next;
            } while (true);
        }

    }

    /// <summary>
    /// 获取该队列球的头球
    /// </summary>
    public Ball Head
    {
        get
        {
            Ball ball = this;
            do
            {
                //Debug.Log(GetType() + "/Head()/");


                if (ball.Pre == null)
                {
                    return ball;
                }
                ball = ball.Pre;
            } while (true);
        }

    }

    public void Recovery() {
        mGameManager.mBallPool.AddObject(this);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 获取当前球前后相同颜色类型的球个数
    /// </summary>
    /// <returns></returns>
    public int SameColorBallCount(out List<Ball> sameColorBallLst) {
        sameColorBallLst = new List<Ball>();
        sameColorBallLst.Add(this);
        // 色球计数开始为 1（本身色球）
        int counter = 1;

        // 前面球同色计数
        Ball curBall = this;
        do
        {
            if (curBall.Pre != null && curBall.Pre.mBallType == mBallType)
            {
                sameColorBallLst.Add(curBall.Pre);
                counter++;
                curBall = curBall.Pre;
            }
            else {
                break;
            }
        } while (curBall != null);

        // 后面球同色计数
        curBall = this;
        do
        {
            if (curBall.Next != null && curBall.Next.mBallType == mBallType)
            {
                sameColorBallLst.Add(curBall.Next);
                counter++;
                curBall = curBall.Next;
            }
            else
            {
                break;
            }
        } while (curBall != null);

        return counter;
    }
}
