using System;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public AudioSource[] audioSource; //オーディオ情報の格納

    void Start()
    {
        audioSource = this.transform.GetComponents<AudioSource>();
    }

    public void SetAudioPitch(float pitch)
    {
        for (int i = 0; i < audioSource.Length; i++)
            audioSource[i].pitch = pitch;
    }

    public void Play(int num)
    {
        if (!audioSource[num].isPlaying) audioSource[num].Play();
    }

    public void Stop(int num)
    {
        if (audioSource[num].isPlaying) audioSource[num].Stop();
    }
}
