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
    int[] sound3array = new int[2];

    void Start()
    {
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        sound3array[0] = 4;
        sound3array[1] = 6;
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
                    if (!_BGMPlayer._AudioSource[lastPlayed].isPlaying)
                        for (int i = 0; i < 2; i++)
                        {
                            if (sound3array[i] != lastPlayed)
                            {
                                lastPlayed = sound3array[i];
                                _BGMPlayer.PlayBGM(lastPlayed);
                                break;
                            }
                        }
                    break;
            }
        }
        else if (manager[0].craneStatus == -1 && manager[1].craneStatus == -1)
        {
            // Nothing to do.
        }
        else
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
