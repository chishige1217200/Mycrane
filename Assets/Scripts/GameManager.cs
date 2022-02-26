using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
}
