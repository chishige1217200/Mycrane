using System;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
	public AudioSource[] _AudioSource; //オーディオ情報の格納
    public int _AudioIndex; //オーディオ数
	public bool[] BGMflag; //BGMを再生するかどうか

	void Start()
	{
		BGMflag = new bool[2];
		_AudioSource = GameObject.Find("BGM").GetComponents<AudioSource>();
		for(int i = 0; i < _AudioIndex; i++)
		{
			BGMflag[i] = false; //すべての再生を無効にする
		}
	}

	void Update()
	{
		for(int i = 0; i < _AudioIndex; i++)
		{
			if(BGMflag[i] == true && _AudioSource[i].isPlaying == false)
			{
				_AudioSource[i].Play();
			}
		}
	}

	public void PlayBGM(int num)
	{
		BGMflag[num] = true;
	}

	public void StopBGM(int num)
	{
		_AudioSource[num].Stop();
		BGMflag[num] = false;
	}
}
