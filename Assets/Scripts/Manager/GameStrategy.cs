using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStrategy : MonoBehaviour
{
    static int mTypeNumber = 0;
    static int mBallType;

    public static BallType SpawnBallStrategy() {
        if (mTypeNumber<=0)
        {
            mTypeNumber = Random.Range(1,3);
            mBallType = Random.Range(0,4);
        }

        mTypeNumber--;
        return (BallType)mBallType;
    }
    public static BallType SpawnShootBallType() {
        return (BallType)Random.Range(0, 5);
    }

    public static int BomdDestroyCount = 5;
    public static int SpawnBallCount(int sceneIndex) {
        return GameData.LevelIndex * 10 + 50;
    }
}
