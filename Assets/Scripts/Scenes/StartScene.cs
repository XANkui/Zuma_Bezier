using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public Button Play_Button;

    private void Awake()
    {
        Play_Button.onClick.AddListener(()=> {

            SceneManager.LoadScene("SelectLevel");
        });
    }
}
