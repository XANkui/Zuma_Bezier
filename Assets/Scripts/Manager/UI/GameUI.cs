using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;
    public GameObject mGameOverPanel;
    public GameObject mGameSuccPanel;

    private void Awake()
    {
        Instance = this;
        mGameOverPanel.transform.Find("Reset_Button").GetComponent<Button>().onClick.AddListener(()=> { 
            GameManager.Instance.StartBack();
            mGameOverPanel.SetActive(false);
        });

        mGameOverPanel.transform.Find("RePlay_Button").GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });

        mGameOverPanel.transform.Find("Home_Button").GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("StartScene");
        });

        mGameSuccPanel.transform.Find("Next_Button").GetComponent<Button>().onClick.AddListener(() => {
            GameData.LevelIndex++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });

        mGameSuccPanel.transform.Find("Home_Button").GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("StartScene");
        });

    }

    public void ShowOverPanel() {
        mGameOverPanel.SetActive(true);
    }

    public void ShowGameSuccPanel()
    {
        mGameSuccPanel.SetActive(true);
    }
}
