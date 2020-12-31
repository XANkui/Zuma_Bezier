using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectLevel : MonoBehaviour
{
    public Button World_1_Button, World_2_Button, World_3_Button;
    private GameObject SmallLevelPanel;
    private Button Back_Button;

    public int mWorldIndex=0, mLevelIndex =0;
    private int World_1_Count = 10;
    private int World_2_Count = 10;
    private void Awake()
    {
        SmallLevelPanel = transform.Find("SmallLevelPanel").gameObject;
        World_1_Button.onClick.AddListener(()=> {
            mWorldIndex = 0;
            SmallLevelPanel.SetActive(true);
        });

        World_2_Button.onClick.AddListener(() => {
            mWorldIndex = 1;
            SmallLevelPanel.SetActive(true);
        });

        World_3_Button.onClick.AddListener(() => {
            mWorldIndex = 2;
            SmallLevelPanel.SetActive(true);
        });

        Back_Button = SmallLevelPanel.transform.Find("Back_Button").GetComponent<Button>();
        Back_Button.onClick.AddListener(() => {
            SmallLevelPanel.SetActive(false);
        });

        Transform list = SmallLevelPanel.transform.Find("List");
        for (int i = 0; i < list.childCount; i++)
        {
            int index = i;
            list.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
            {
                mLevelIndex = index;
                if (mWorldIndex == 0)
                {
                    Debug.Log(GetType()+ "/<Button>().onClick()/ level " + mLevelIndex);
                    GameData.LevelIndex = mLevelIndex;
                }
                else if (mWorldIndex == 1)
                {
                    Debug.Log(GetType() + "/<Button>().onClick()/ level " + (World_1_Count + mLevelIndex));
                    GameData.LevelIndex = World_1_Count + mLevelIndex;
                }
                else if (mWorldIndex == 2)
                {
                    Debug.Log(GetType() + "/<Button>().onClick()/ level " + (World_1_Count + World_2_Count  + mLevelIndex));
                    GameData.LevelIndex = World_1_Count + World_2_Count+mLevelIndex;
                }

                SceneManager.LoadScene("Game");
            });
        }

        
    }
}
