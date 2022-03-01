using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] SceneObject scene;
    [SerializeField] new AudioSource audio;
    [SerializeField] bool exitKeyOn = false;

    void Update()
    {
        if (exitKeyOn && (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.KeypadMinus))) SceneTransition();
    }

    public void SceneTransition()
    {
        if (audio != null) audio.PlayOneShot(audio.clip);
        SceneManager.LoadScene(scene);
    }
}
