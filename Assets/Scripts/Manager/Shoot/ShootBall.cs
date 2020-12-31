using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBall : MonoBehaviour
{
    public float mSpeed = 10;
    public BallType mBallType;
    public void Init(BallType ballType, Sprite sp,Transform trans) {
        mBallType = ballType;
        GetComponent<SpriteRenderer>().sprite = sp;
        transform.position = trans.position;
        transform.rotation = trans.rotation;
        this.gameObject.SetActive(true);
    }

    public void Move()
    {
        transform.localPosition += transform.up * Time.deltaTime * mSpeed;
    }

    /// <summary>
    /// 是否超出边界
    /// </summary>
    /// <returns></returns>
    public bool IsOutOfBounds() {
        if (transform.localPosition.x > 3 || transform.localPosition.x < -3 
            || transform.localPosition.y > 5 || transform.localPosition.y < -5)
        {
            return true;
        }

        return false;
    }

    public bool IsCross(Vector3 targetPos, float distance) {
        return Vector3.Distance(transform.position, targetPos) <= distance;
    }
}
