using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneConfig : MonoBehaviour
{
    public Sprite[] bgSpriteArr;
    public MapConfig[] mapConfigArr;
    public Vector3[] shooterPosArr;
    public SpriteRenderer bgRender;
    public Transform shooter;

    private void Awake()
    {
        Sprite sp = bgSpriteArr[GameData.LevelIndex];
        bgRender.sprite = sp;
        MapConfig mapConfig = mapConfigArr[GameData.LevelIndex];
        GetComponent<GameManager>().mMapConfig = mapConfig;
        shooter.position = shooterPosArr[GameData.LevelIndex];
    }
}
