using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    private static void Play(string clipName) {
        AudioClip clip = GetAudioClip(clipName);
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
    }

    public static AudioClip GetAudioClip(string clipName) { 
        return Resources.Load<AudioClip>("Sound/" + clipName); ;
    }

    public static void PlayDestroy() { Play("Eliminate"); }
    public static void PlayInsert() { Play("BallEnter"); }
    public static void PlayBomb() { Play("Bomb"); }
    public static void PlayFail() { Play("Fail"); }
    public static void PlayFastMove() { Play("FastMove"); }
    public static void PlayShoot() { Play("Shoot"); }
    public static void PlayBGMusic(GameObject gameObject) {
        AudioSource audio = gameObject.GetComponent<AudioSource>();
        if (audio==null)
        {
            audio = gameObject.AddComponent<AudioSource>();
        }
        audio.clip = GetAudioClip("Bg");
        audio.loop = true;
        audio.volume = 0.3f;
        audio.Play();
    }
}
