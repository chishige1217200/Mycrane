using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool isMenu = false;
    void Start()
    {
        if (isMenu)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    //　ゲーム終了ボタンを押したら実行する
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
                Application.OpenURL("https://sites.google.com/view/mycrane");
#else
        Application.Quit();
#endif
    }

    public void OpenHome()
    {
        Application.OpenURL("https://sites.google.com/view/mycrane");
    }

    public void OpenYouTube()
    {
        Application.OpenURL("https://www.youtube.com/channel/UCs5Z8JDmjCh__vymU6BdMWQ");
    }

    public void OpenTwitter()
    {
        Application.OpenURL("https://twitter.com/mycraneofficial");
    }

    public void ResetCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
