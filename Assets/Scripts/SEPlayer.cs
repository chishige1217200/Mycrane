using System;
using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    public AudioSource[] audioSources; //オーディオ情報の格納
    private int[] repeatCount; //リピート再生回数

    void Start()
    {
        audioSources = transform.GetComponents<AudioSource>();
        repeatCount = new int[audioSources.Length];
        for (int i = 0; i < audioSources.Length; i++)
        {
            repeatCount[i] = 0; //すべてのリピート再生回数を0にする
        }
    }

    void Update()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (repeatCount[i] > 0 && !audioSources[i].isPlaying)
            {
                audioSources[i].PlayOneShot(audioSources[i].clip);
                repeatCount[i]--;
            }
        }
    }

    public void SetAudioPitch(float pitch)
    {
        for (int i = 0; i < audioSources.Length; i++)
            audioSources[i].pitch = pitch;
    }

    public void Play(int num, int repeatcount)
    {
        audioSources[num].loop = false;
        repeatCount[num] = repeatcount;
    }

    public void Play(int num)
    {
        audioSources[num].loop = true;
        if (!audioSources[num].isPlaying) audioSources[num].Play();
    }

    public void Stop(int num)
    {
        audioSources[num].Stop();
        repeatCount[num] = 0;
    }

    public void ForcePlay(int num)
    {
        audioSources[num].Stop();
        repeatCount[num] = 0;
        audioSources[num].Play();
    }
}
