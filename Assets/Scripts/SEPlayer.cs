using System;
using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    public AudioSource[] audioSource; //オーディオ情報の格納
    private int[] repeatCount; //リピート再生回数

    void Start()
    {
        audioSource = this.transform.GetComponents<AudioSource>();
        repeatCount = new int[audioSource.Length];
        for (int i = 0; i < audioSource.Length; i++)
        {
            repeatCount[i] = 0; //すべてのリピート再生回数を0にする
        }
    }

    void Update()
    {
        for (int i = 0; i < audioSource.Length; i++)
        {
            if (repeatCount[i] > 0 && !audioSource[i].isPlaying)
            {
                audioSource[i].PlayOneShot(audioSource[i].clip);
                repeatCount[i]--;
            }
        }
    }

    public void SetAudioPitch(float pitch)
    {
        for (int i = 0; i < audioSource.Length; i++)
            audioSource[i].pitch = pitch;
    }

    public void Play(int num, int repeatcount)
    {
        repeatCount[num] = repeatcount;
    }

    public void Play(int num)
    {
        repeatCount[num] = 2147483647;
    }

    public void Stop(int num)
    {
        audioSource[num].Stop();
        repeatCount[num] = 0;
    }

    public void ForcePlay(int num)
    {
        audioSource[num].Stop();
        repeatCount[num] = 0;
        audioSource[num].Play();
    }
}
