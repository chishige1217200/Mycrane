using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type5Selecter : MonoBehaviour
{
    [SerializeField] Type5Manager[] manager = new Type5Manager[2];
    [SerializeField] Animator[] animator = new Animator[3];
    BGMPlayer _BGMPlayer;
    [SerializeField] int soundType = 0; //0:U9_1，1:U9_2，2:U8，3:U7
    [SerializeField] float audioPitch = 1.0f;
    [SerializeField] int lightColor = 1; //1:Red, 2:Green, 3:Blue, 4:Yellow, 5:Sky, 6:Purple, 7:Pink(255,128,255), 8:Orange(255,128,0), 9:Forest(128,255,0)
    int nextPlay = 6;

    void Start()
    {
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _BGMPlayer.SetAudioPitch(audioPitch);
        manager[0].soundType = soundType;
        manager[1].soundType = soundType;
        manager[0]._SEPlayer.SetAudioPitch(audioPitch);
        manager[1]._SEPlayer.SetAudioPitch(audioPitch);

        for (int i = 0; i < 3; i++)
        {
            animator[i].SetInteger("ColorNumber", lightColor);
            manager[0].animator[i] = animator[i];
            manager[1].animator[i] = animator[i];
        }
    }

    void Update()
    {
        if (manager[0].craneStatus == 0 && manager[1].craneStatus == 0)
        {
            switch (soundType)
            {
                case 0:
                    _BGMPlayer.StopBGM(1);
                    _BGMPlayer.PlayBGM(0);
                    break;
                case 1:
                    _BGMPlayer.StopBGM(3);
                    _BGMPlayer.PlayBGM(2);
                    break;
                case 2:
                    _BGMPlayer.StopBGM(5);
                    _BGMPlayer.PlayBGM(4);
                    break;
                case 3:
                    _BGMPlayer.StopBGM(8);
                    if (!_BGMPlayer._AudioSource[6].isPlaying && !_BGMPlayer._AudioSource[7].isPlaying)
                    {
                        if (nextPlay == 6)
                        {
                            _BGMPlayer._AudioSource[6].Play(); //1回しか再生したくないため
                            nextPlay = 7;
                        }
                        else if (nextPlay == 7)
                        {
                            _BGMPlayer._AudioSource[7].Play(); //1回しか再生したくないため
                            nextPlay = 6;
                        }
                    }
                    break;
            }
        }
        else if (manager[0].craneStatus > 0 || manager[1].craneStatus > 0)
        {
            switch (soundType)
            {
                case 0:
                    _BGMPlayer.StopBGM(0);
                    _BGMPlayer.PlayBGM(1);
                    break;
                case 1:
                    _BGMPlayer.StopBGM(2);
                    _BGMPlayer.PlayBGM(3);
                    break;
                case 2:
                    _BGMPlayer.StopBGM(4);
                    _BGMPlayer.PlayBGM(5);
                    break;
                case 3:
                    _BGMPlayer.StopBGM(6);
                    _BGMPlayer.StopBGM(7);
                    _BGMPlayer.PlayBGM(8);
                    break;
            }
            /*if (_BGMPlayer._AudioSource[0].isPlaying) await Task.Delay(500);
            if (!_BGMPlayer._AudioSource[1].isPlaying && (!_SEPlayer[0]._AudioSource[6].isPlaying || !_SEPlayer[1]._AudioSource[6].isPlaying))
            {
                _BGMPlayer.StopBGM(0);
                _BGMPlayer.PlayBGM(1);
            }*/
        }
    }

    public Type5Manager GetManager(int num)
    {
        return manager[num - 1];
    }
}
