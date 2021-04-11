using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlay : MonoBehaviour
{
    public VideoPlayer[] _VideoSource; //オーディオ情報の格納
    public bool randomMode = false; //trueのときランダムにビデオを再生
    private int[] _RepeatCount; //リピート再生回数

    void Start()
    {
        _VideoSource = this.transform.GetComponents<VideoPlayer>();
        _RepeatCount = new int[_VideoSource.Length];
        for (int i = 0; i < _VideoSource.Length; i++)
        {
            _RepeatCount[i] = 0; //すべてのリピート再生回数を0にする
        }
    }

    void Update()
    {
        for (int i = 0; i < _VideoSource.Length; i++)
        {
            if (_RepeatCount[i] > 0 && _VideoSource[i].isPlaying == false)
            {
                _VideoSource[i].enabled = true;
                _VideoSource[i].Play();
                _RepeatCount[i]--;
            }
        }
    }

    public void PlayVideo(int num, int repeatcount)
    {
        _RepeatCount[num] = repeatcount;
    }

    public void StopVideo(int num)
    {
        _VideoSource[num].enabled = false;
        _VideoSource[num].Stop();
        _RepeatCount[num] = 0;
    }
}
