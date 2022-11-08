using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class NoOperationChecker : MonoBehaviour
{
    SceneChanger sc;
    VideoPlayer vp;
    private IEnumerator DelayCoroutine(float miliseconds, Action action)
    {
        yield return new WaitForSeconds(miliseconds / 1000f);
        action?.Invoke();
    }
    // Start is called before the first frame update
    void Start()
    {
        sc = GetComponent<SceneChanger>();
        StartCoroutine(DelayCoroutine(5000, () =>
        {
            vp = GetComponent<VideoPlayer>();
        }));
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey || (vp != null && !vp.isPlaying)) sc.SceneTransition();
    }
}
