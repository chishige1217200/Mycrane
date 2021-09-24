using System;
using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    public AudioSource[] _AudioSource; //オーディオ情報の格納
    //public int _AudioIndex; //オーディオ数
    private int[] _RepeatCount; //リピート再生回数

    void Start()
    {
        _AudioSource = this.transform.GetComponents<AudioSource>();
        _RepeatCount = new int[_AudioSource.Length];
        for (int i = 0; i < _AudioSource.Length; i++)
        {
            _RepeatCount[i] = 0; //すべてのリピート再生回数を0にする
        }
    }

    void Update()
    {
        for (int i = 0; i < _AudioSource.Length; i++)
        {
            if (_RepeatCount[i] > 0 && _AudioSource[i].isPlaying == false)
            {
                _AudioSource[i].PlayOneShot(_AudioSource[i].clip);
                _RepeatCount[i]--;
            }
        }
    }

    public void SetAudioPitch(float pitch)
    {
        for (int i = 0; i < _AudioSource.Length; i++)
            _AudioSource[i].pitch = pitch;
    }

    public void PlaySE(int num, int repeatcount)
    {
        _RepeatCount[num] = repeatcount;
    }

    public void Play(int num)
    {
        _RepeatCount[num] = 2147483647;
    }

    public void StopSE(int num)
    {
        _AudioSource[num].Stop();
        _RepeatCount[num] = 0;
    }

    public void ForcePlaySE(int num)
    {
        _AudioSource[num].Stop();
        _RepeatCount[num] = 0;
        _AudioSource[num].Play();
    }
}
