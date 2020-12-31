using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleOnce : MonoBehaviour
{
    private MonoBehaviour mono;
    private IEnumerator doingIE;
    public static ScheduleOnce Start(MonoBehaviour mono, System.Action doing, float time) {
        return new ScheduleOnce(mono, doing, time);
    }

    public void Stop()
    {
        if (doingIE !=null)
        {
            mono.StopCoroutine(doingIE);
            doingIE = null;
        }
    }

    public bool IsDoing() {
        return doingIE != null;
    }

    private ScheduleOnce(MonoBehaviour mono, System.Action doing, float time) {
        this.mono = mono;
        doingIE = WaitSomeTimeToDO(time, doing);
        mono.StartCoroutine(doingIE);
    }

    private IEnumerator WaitSomeTimeToDO(float time, System.Action doing) {
        yield return new WaitForSeconds(time);
        doing();
        doingIE = null;
    }
}
