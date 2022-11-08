using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrasJewelSelecter : MonoBehaviour
{
    [SerializeField] TetrasJewelManager[] manager = new TetrasJewelManager[2];
    BGMPlayer bp;
    [SerializeField] int soundType = 0; //0 - 1
    [SerializeField] float audioPitch = 1.0f;

    void Start()
    {
        bp = transform.Find("BGM").GetComponent<BGMPlayer>();
        bp.SetAudioPitch(audioPitch);
        manager[0].sp.SetAudioPitch(audioPitch);
        manager[1].sp.SetAudioPitch(audioPitch);
    }

    void Update()
    {
        if (manager[0].craneStatus == 0 && manager[1].craneStatus == 0)
        {
            bp.Stop(2);
            bp.Play(soundType);
        }
        else if (manager[0].craneStatus > 0 || manager[1].craneStatus > 0)
        {
            bp.Stop(soundType);
            bp.Play(2);
        }
    }
}
