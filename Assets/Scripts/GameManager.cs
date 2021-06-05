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
                Application.OpenURL("http://www.yahoo.co.jp/");
#else
        Application.Quit();
#endif
    }

    public void GoScene(int num)
    {
        switch (num)
        {
            case 0:
                SceneManager.LoadScene("Title");
                break;
            case 1:
                SceneManager.LoadScene("KroonTest");
                break;
            case 2:
                SceneManager.LoadScene("Type2Test");
                break;
            case 3:
                SceneManager.LoadScene("Type3Test");
                break;
            case 4:
                SceneManager.LoadScene("Type4Test");
                break;
            case 5:
                SceneManager.LoadScene("Type5Test");
                break;
            case 7:
                SceneManager.LoadScene("Type7Test");
                break;
        }
    }
}
