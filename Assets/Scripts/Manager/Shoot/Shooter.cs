using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Shooter : MonoBehaviour
{

    private SpriteRenderer mShootBallSpRd;
    private BallType mCurBallType;

    // Start is called before the first frame update
    void Start()
    {
        mShootBallSpRd = transform.Find("Ball").GetComponent<SpriteRenderer>();
        RefreshBallType();
    }

    // Update is called once per frame
    void Update()
    {
        // 点击UI不发射子弹
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float y = mousePos.y - transform.position.y;
            float x = mousePos.x - transform.position.x;

            // 弧度角 弧度转角度
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0,0,angle - 90);    // 补差
        }

        if (Input.GetMouseButtonUp(0))
        {
            // 发射球
            shoot();
            
        }
    }

    private void shoot() {
        if (mShootBallSpRd.gameObject.activeSelf==false)
        {
            return;
        }
        SoundManager.PlayShoot();
        ShootBallManager.Instance.Shoot(mCurBallType, mShootBallSpRd.sprite,transform);
        mShootBallSpRd.gameObject.SetActive(false);
        // 形成发射球的效果
        //Invoke("RefreshBallType",0.2f);
        // 延时 0.5f 执行回收
        ScheduleOnce.Start(this, RefreshBallType, 0.5f);
    }

    private void RefreshBallType() {
        //Debug.Log(GetType() + "/RefreshBallType()/");
        mCurBallType = GameStrategy.SpawnShootBallType();
        mShootBallSpRd.sprite = GameManager.Instance.GetBallTypeSprite(mCurBallType);
        mShootBallSpRd.gameObject.SetActive(true);
    }
}
