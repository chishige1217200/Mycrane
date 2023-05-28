using System;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public AudioSource[] audioSources; //オーディオ情報の格納

    void Start()
    {
        audioSources = transform.GetComponents<AudioSource>();
    }

    public void SetAudioPitch(float pitch)
    {
        for (int i = 0; i < audioSources.Length; i++)
            audioSources[i].pitch = pitch;
    }

    public void Play(int num)
    {
        if (!audioSources[num].isPlaying) audioSources[num].Play();
    }

    public void Stop(int num)
    {
        if (audioSources[num].isPlaying) audioSources[num].Stop();
    }
}
