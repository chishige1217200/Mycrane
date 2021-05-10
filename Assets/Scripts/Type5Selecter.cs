using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type5Selecter : MonoBehaviour
{
    [SerializeField] Type5Manager[] manager = new Type5Manager[2];
    BGMPlayer _BGMPlayer;
    public int soundType = 0;
    int lastPlayed = 6;
    bool updateFlag = true; //trueなら更新可能 soundType2用

    void Start()
    {
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        manager[0].soundType = soundType;
        manager[1].soundType = soundType;
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
                    _BGMPlayer.StopBGM(6);
                    if (!_BGMPlayer._AudioSource[4].isPlaying && !_BGMPlayer._AudioSource[6].isPlaying)
                    {
                        if (lastPlayed == 4)
                        {
                            _BGMPlayer._AudioSource[6].Play(); //1回しか再生したくないため
                            lastPlayed = 6;
                        }
                        else if (lastPlayed == 6)
                        {
                            _BGMPlayer._AudioSource[4].Play(); //1回しか再生したくないため
                            lastPlayed = 4;
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
                    _BGMPlayer.StopBGM(5);
                    _BGMPlayer.PlayBGM(6);
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
