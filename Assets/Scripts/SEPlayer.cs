using System;
using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    public static AudioSource[] _AudioSource; //オーディオ情報の格納
    public int _AudioIndex; //オーディオ数
    private static int[] _RepeatCount; //リピート再生回数

    void Start()
    {
        _RepeatCount = new int[_AudioIndex];
        _AudioSource = GameObject.Find("SE").GetComponents<AudioSource>();
        for (int i = 0; i < _AudioIndex; i++)
        {
            _RepeatCount[i] = 0; //すべてのリピート再生回数を0にする
        }
    }

    void Update()
    {
        for (int i = 0; i < _AudioIndex; i++)
        {
            if (_RepeatCount[i] > 0 && _AudioSource[i].isPlaying == false)
            {
                _AudioSource[i].PlayOneShot(_AudioSource[i].clip);
                _RepeatCount[i]--;
            }
        }
    }

    public void PlaySE(int num, int repeatcount)
    {
        _RepeatCount[num] = repeatcount;
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