using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type6Selecter : MonoBehaviour
{
    [SerializeField] Type6Manager[] manager = new Type6Manager[2];
    [SerializeField] Animator[] animator = new Animator[3];
    BGMPlayer _BGMPlayer;
    [SerializeField] int soundType = 0; //0:U9_1，1:U9_2
    [SerializeField] float audioPitch = 1.0f;
    [SerializeField] int lightColor = 1; //1:Red, 2:Green, 3:Blue, 4:Yellow, 5:Sky, 6:Purple, 7:Pink(255,128,255), 8:Orange(255,128,0), 9:Forest(128,255,0)

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
                    _BGMPlayer.Stop(1);
                    _BGMPlayer.Play(0);
                    break;
                case 1:
                    _BGMPlayer.Stop(3);
                    _BGMPlayer.Play(2);
                    break;
            }
        }
        else if (manager[0].craneStatus > 0 || manager[1].craneStatus > 0)
        {
            switch (soundType)
            {
                case 0:
                    _BGMPlayer.Stop(0);
                    _BGMPlayer.Play(1);
                    break;
                case 1:
                    _BGMPlayer.Stop(2);
                    _BGMPlayer.Play(3);
                    break;
            }
        }
    }

    public Type6Manager GetManager(int num)
    {
        return manager[num - 1];
    }
}
