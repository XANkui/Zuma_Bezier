using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BallType
{ 
    Red,
    Blue,
    Yellow,
    Green,
    Bomb
}

[System.Serializable]
public class BallTypeSprite {
    public BallType ballType;
    public Sprite sprite;
}

public enum GameState { 
    Game,
    Fail,
    Succ
}

public enum MoveState { 
    Forward,
    Back
}

public class GameManager : MonoBehaviour
{
    public MapConfig mMapConfig;
    public GameObject mBallPrefab;
    public List<BallTypeSprite> mBallTypeSpriteLst = new List<BallTypeSprite>();
    public float mMoveSpeed = 2f;

    public static GameManager Instance;

    private bool mIsBomb = false;
    private GameState mGameState = GameState.Game;
    private MoveState mMoveState = MoveState.Forward;

    public ObjectPool<Ball> mBallPool;

    /// <summary>
    /// 球队列 段 集合，存放的是首位球
    /// </summary>
    private List<Ball> mBallSegmentLst = new List<Ball>();

    /// <summary>
    /// 判断插入后是否连续同色消除的球集合
    /// </summary>
    private List<Ball> mSearchDestroyBallLst = new List<Ball>();

    /// <summary>
    /// 需要回退的球集合
    /// </summary>
    private List<Ball> mFallbackBallLst = new List<Ball>();

    private Dictionary<BallType, Sprite> mBallTypeSpriteDic;

    public Sprite GetBallTypeSprite(BallType ballType) {
        if (mBallTypeSpriteDic.ContainsKey(ballType) ==false)
        {
            return null;
        }

        return mBallTypeSpriteDic[ballType];
    }

    private void Awake()
    {
        Instance = this;

        mBallTypeSpriteDic = new Dictionary<BallType, Sprite>();

        foreach (var item in mBallTypeSpriteLst)
        {
            mBallTypeSpriteDic.Add(item.ballType, item.sprite);
        }

        
        mBallPool = new ObjectPool<Ball>(InstanceBallFunc, 10);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        mMapConfig.InitMapConfig();

        SoundManager.PlayBGMusic(this.gameObject);

        mMoveSpeed = 10;
        SoundManager.PlayFastMove();
        yield return new WaitForSeconds(2);
        mMoveSpeed = 2;
    }

    private Ball InstanceBallFunc() {
        GameObject ball = Instantiate(mBallPrefab, transform.Find("BallPool"));
        ball.SetActive(false);
        Ball ballScript = ball.AddComponent<Ball>();
        ballScript.SpawnInit();
        return ballScript;
    }

    private void Update()
    {
        if (mGameState==GameState.Game)
        {
            if (mMoveState == MoveState.Forward)
            {
                FirstSegmentMove();
                CheckGameFail();
            }
            else {
                SegmentBack();
            }
            
            ShootBallInsert();
            SearchDestroyBall();
            CheckFallbackBall();
            BallSegmentConnect();
        }


        
    }


    /// <summary>
    /// 第一段移动
    /// </summary>
    private void FirstSegmentMove() {
        int spawnCount = GameStrategy.SpawnBallCount(SceneManager.GetActiveScene().buildIndex);

        if (mBallSegmentLst.Count == 0 && mBallPool.Counter < spawnCount)
        {
            mBallPool.Counter++;

            Ball ball = mBallPool.GetObject().Init(this, GameStrategy.SpawnBallStrategy());
            mBallSegmentLst.Add(ball);
            return;
        }

        if (mBallSegmentLst.Count<=0)
        {
            Debug.Log(GetType()+"/()/ ball clear, game win");
            mGameState = GameState.Succ;
            GameUI.Instance.ShowGameSuccPanel();
            return;
        }

        Ball fb = mBallSegmentLst[0];
        // 如果第一段第一个球已经出洞口，填充新的球洞口
        if (fb.IsExitStartHole && mBallPool.Counter < spawnCount)
        {
            mBallPool.Counter++;

            Ball ball = mBallPool.GetObject().Init(this, GameStrategy.SpawnBallStrategy());
            ball.mProgress = 0;
            ball.Next = fb;
            fb.Pre = ball;

            mBallSegmentLst[0] = ball;
            fb = ball;
        }

        fb.mProgress += Time.deltaTime * mMoveSpeed;

        while (fb.Next !=null)
        {
            if (fb.Next.mProgress < fb.mProgress +1)
            {
                fb.Next.mProgress = fb.mProgress + 1;
            }

            fb = fb.Next;
        }
    }

    private void CheckGameFail() {
        int count = mBallSegmentLst.Count;
        if (count==0)
        {

            return;
        }

        Ball fb = mBallSegmentLst[count-1];

        if (fb.Tail.IsArriveFailHole)
        {
            Debug.Log(GetType() + "/CheckGameFail()/ is Game Over");
            SoundManager.PlayFail();
            mGameState = GameState.Fail;
            GameUI.Instance.ShowOverPanel();
        }
    }

    public void ShootBallInsert() {

        float distance = 0.3f;

        List<ShootBall> shootBallLst = ShootBallManager.Instance.mShootBallLst;

        int i = shootBallLst.Count;
        while (i-->0)
        {
            bool isHit = false;
            ShootBall shootBall = shootBallLst[i];
            int j = mBallSegmentLst.Count;
            while (j-- > 0)
            {
                Ball fb = mBallSegmentLst[j];
                do
                {
                    if (shootBall.IsCross(fb.transform.position, distance))
                    {
                        if (shootBall.mBallType != BallType.Bomb)
                        {
                            // 表示找到了距离射击球最近的点
                            Ball insert = mBallPool.GetObject().Init(this, shootBall.mBallType);
                            Ball next = fb.Next;
                            fb.Next = insert;
                            insert.Pre = fb;
                            insert.Next = next;
                            if (next != null)
                            {
                                next.Pre = insert;
                            }
                            insert.mProgress = fb.mProgress + 1;
                            mSearchDestroyBallLst.Add(insert);
                            isHit = true;
                            
                        }//炸弹球的情况
                        else {
                            fb.mIsDestory = true;
                            Ball ball = fb.Pre;
                            int count = GameStrategy.BomdDestroyCount/2;
                            while (ball !=null && count-->0)
                            {
                                ball.mIsDestory = true;
                                ball = ball.Pre;
                            }

                            ball = fb.Next;
                            count = GameStrategy.BomdDestroyCount/2;
                            while (ball != null && count-- > 0)
                            {
                                ball.mIsDestory = true;
                                ball = ball.Next;
                            }
                            mIsBomb = true;
                            SoundManager.PlayBomb();
                        }

                        // 把标记的球移除回收
                        shootBallLst.RemoveAt(i);
                        ShootBallManager.Instance.Recovery(shootBall);
                        break;

                    }

                    fb = fb.Next;
                } while (fb!=null);

                if (isHit == true)
                {
                    UpdateBallProgress(mBallSegmentLst[j]);
                    break;
                }
                if (mIsBomb)
                {
                    break; 
                }
            }

            
        }

        
    }

    public void SearchDestroyBall() {
        int i = mSearchDestroyBallLst.Count;
        bool isSame = false;
        while (i-->0)
        {
            List<Ball> sameColorBallLst;
            Ball ball = mSearchDestroyBallLst[i];
            if (ball.SameColorBallCount(out sameColorBallLst) >= 3)
            {
                isSame = true;
                foreach (var item in sameColorBallLst)
                {
                    item.mIsDestory = true;
                }
                SoundManager.PlayDestroy();
            }
            else {
                SoundManager.PlayInsert();
            }
        }

        mSearchDestroyBallLst.Clear();

        if (isSame == false && mIsBomb ==false)
        {
            return;
        }

        // 炸弹球执行一次回复
        mIsBomb = false;

        int x = mBallSegmentLst.Count;
        while (x-->0)
        {
            Ball fb = mBallSegmentLst[x];

            Ball head = fb.Head;
            Ball tail = fb.Tail;
            bool isDelete = false;
            do
            {
                if (fb.mIsDestory)
                {
                    isDelete = true;

                    #region 切断连接
                    if (fb.Pre != null)
                    {
                        fb.Pre.Next = null;
                    }

                    if (fb.Next != null)
                    {
                        fb.Next.Pre = null;
                    }
                    #endregion

                    // 头球和尾球销毁的情况
                    if (head==fb)
                    {
                        head = null;
                    }
                    if (tail == fb)
                    {
                        tail = null;
                    }

                    FXManager.Instance.ShowDestroyFX(fb.transform.position);

                    fb.Recovery();
                }
                fb = fb.Next;
                
            } while (fb != null);


            if (isDelete==false)
            {
                continue;
            }

            // 处理分裂
            if (head != null)
            {
                mBallSegmentLst[x] = head;
                if (tail != null)
                {
                    mBallSegmentLst.Insert(x + 1, tail.Head);
                }
            }
            else {
                if (tail != null)
                {
                    mBallSegmentLst[x] = tail.Head;
                }
                else {
                    mBallSegmentLst.RemoveAt(x);
                }
            }

            // 回退
            Ball target = null;
            if (head !=null)
            {
                target = head.Tail;
            }
            else if (x > 0)
            {
                target = mBallSegmentLst[x - 1].Tail;
            }
            if (target !=null)
            {
                Ball p = null;
                if (tail != null)
                {
                    p = tail.Head;
                }
                else if (x + 1<= mBallSegmentLst.Count-1)
                {
                    p = mBallSegmentLst[x+1];                    
                }

                if (p!=null && p.mBallType == target.mBallType)
                {
                    p.mFallbackBall = target;
                    mFallbackBallLst.Add(p);
                }
                
            }

        }
    }

    /// <summary>
    /// 处理检测有回退趋势的球
    /// </summary>
    private void CheckFallbackBall() {
        int i = mFallbackBallLst.Count;

        while (i-->0)
        {
            Ball ball = mFallbackBallLst[i];

            if (ball.gameObject.activeSelf == false || ball.mFallbackBall.gameObject.activeSelf==false)
            {
                mFallbackBallLst.RemoveAt(i);
                continue;
            }

            ball.mProgress -= Time.deltaTime * 15;
            UpdateBallProgress(ball);

            // 代表回退到FallbackTarget位置了
            if (ball.mProgress <= ball.mFallbackBall.mProgress+1)
            {
                mSearchDestroyBallLst.Add(ball);
                mFallbackBallLst.RemoveAt(i);
            }
        }
    }

    private void UpdateBallProgress(Ball ball) {
        while (ball!=null)
        {
            if (ball.Next != null)
            {
                ball.Next.mProgress = ball.mProgress + 1;
            }

            ball = ball.Next;
        }
    }

    /// <summary>
    /// 处理球段之间的连接
    /// </summary>
    private void BallSegmentConnect() {
        int i = mBallSegmentLst.Count;

        while (i-->1)
        {
            Ball nextSeg = mBallSegmentLst[i];
            Ball preSeg = mBallSegmentLst[i - 1];
            Ball tail = preSeg.Tail;

            if (tail.mProgress >= nextSeg.mProgress -1)
            {
                nextSeg.mProgress = tail.mProgress + 1;
                UpdateBallProgress(nextSeg);
                tail.Next = nextSeg;
                nextSeg.Pre = tail;
                mBallSegmentLst.RemoveAt(i);
            }
        }
    }

    private void SegmentBack() {
        int i = mBallSegmentLst.Count;
        if (i<=0)
        {
            return;
        }

        Ball p = mBallSegmentLst[i - 1].Tail;
        p.mProgress -= Time.deltaTime * 10;
        while (p.Pre != null)
        {
            if (p.Pre.mProgress > p.mProgress-1)
            {
                p.Pre.mProgress = p.mProgress - 1;
            }
            p = p.Pre;

            // 退出出洞口
            if (p.mProgress <0)
            {
                mBallSegmentLst[i - 1] = p.Next;
                p.Next.Pre = null;

                do
                {
                    // 回退数量减去，后面重新生成
                    mBallPool.Counter--;

                    p.Recovery();
                    p = p.Pre;
                } while (p!=null);

                break;
            }
        }
    }

    /// <summary>
    /// 开始回退
    /// </summary>
    public void StartBack() {
        mGameState = GameState.Game;
        mMoveState = MoveState.Back;
        ScheduleOnce.Start(this,()=> {
            mMoveState = MoveState.Forward;
        },2.0f);
    }
}
