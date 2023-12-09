using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] SceneObject scene;
    [SerializeField] new AudioSource audio;
    LoadingUIController loadingUIController;
    [SerializeField] bool exitKeyOn = false;

    void Start()
    {
        GameObject gb = GameObject.Find("GameManager");
        if (gb != null && gb.TryGetComponent(out loadingUIController))
        {
            loadingUIController = gb.GetComponent<LoadingUIController>();
        }
    }

    void Update()
    {
        if (exitKeyOn && (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.KeypadMinus))) SceneTransition();
    }

    public void SceneTransition()
    {
        StartCoroutine(DoTransit());
    }

    private IEnumerator DoTransit()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        // async.allowSceneActivation = false;
        if (loadingUIController != null)
        {
            loadingUIController.Show();
        }

        if (audio != null)
        {
            audio.PlayOneShot(audio.clip);
            async.allowSceneActivation = false;
        }

        StartCoroutine(UpdateProgress(async));

        // if (audio != null)
        // {
        //     while (audio.isPlaying)
        //     {
        //         yield return null;
        //     }
        // }

        if (audio != null) yield return new WaitForSeconds(audio.clip.length);

        async.allowSceneActivation = true;

        yield return async;
    }

    private IEnumerator UpdateProgress(AsyncOperation async)
    {
        while (!async.isDone)
        {
            if (loadingUIController != null)
            {
                loadingUIController.SetProgress(async.progress);
            }
            yield return null;
        }

        if (loadingUIController != null)
        {
            loadingUIController.SetProgress(1);
        }
    }
}
