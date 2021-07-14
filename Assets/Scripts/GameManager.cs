using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //　ゲーム終了ボタンを押したら実行する
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
                Application.OpenURL("http://www.google.com");
#else
        Application.Quit();
#endif
    }

    public void GoScene(int num)
    {
        switch (num)
        {
            case 0:
                SceneManager.LoadSceneAsync("Title");
                break;
            case 1:
                SceneManager.LoadSceneAsync("TakoyakiTest");
                break;
            case 2:
                SceneManager.LoadSceneAsync("Type2Test");
                break;
            case 3:
                SceneManager.LoadSceneAsync("Type3Test");
                break;
            case 4:
                SceneManager.LoadSceneAsync("Type4Test");
                break;
            case 5:
                SceneManager.LoadSceneAsync("Type5Test");
                break;
            case 7:
                SceneManager.LoadSceneAsync("Type7Test");
                break;
            case 100:
                SceneManager.LoadSceneAsync("GameCenterTest");
                break;
        }
    }
}
