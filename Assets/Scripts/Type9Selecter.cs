using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type9Selecter : MonoBehaviour
{
    [SerializeField] Type9Manager[] manager = new Type9Manager[4];
    BGMPlayer _BGMPlayer;
    [SerializeField] int soundType = 0; //0，1，2

    void Start()
    {
        _BGMPlayer = transform.Find("BGM").GetComponent<BGMPlayer>();
    }

    void Update()
    {
        if (manager[0].GetStatus() >= 0 && manager[1].GetStatus() >= 0 && manager[2].GetStatus() >= 0 && manager[3].GetStatus() >= 0)
        {
            if (!IsPlaying())
            {
                _BGMPlayer.StopBGM(3);
                _BGMPlayer.PlayBGM(soundType);
            }
            else
            {
                _BGMPlayer.StopBGM(soundType);
                _BGMPlayer.PlayBGM(3);
            }
        }
    }

    bool IsPlaying()
    {
        if (manager[0].GetStatus() == 0 && manager[1].GetStatus() == 0 && manager[2].GetStatus() == 0 && manager[3].GetStatus() == 0) return false;
        else return true;
    }
}
