using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] SceneObject scene;
    [SerializeField] new AudioSource audio;

    public void SceneTransition()
    {
        if (audio != null) audio.PlayOneShot(audio.clip);
        SceneManager.LoadScene(scene);
    }
}
