using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type1Selecter : MonoBehaviour
{
    [SerializeField] Type1Manager[] manager = new Type1Manager[2];
    [SerializeField] int soundType = 0;
    BGMPlayer _BGMPlayer;

    void Start()
    {
        manager[0].soundType = soundType;
        manager[1].soundType = soundType;
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
    }

    void Update()
    {
        if (manager[0].craneStatus >= 0 && manager[1].craneStatus >= 0 && !_BGMPlayer._AudioSource[soundType].isPlaying)
            _BGMPlayer.Play(soundType);
    }

    public Type1Manager GetManager(int num)
    {
        return manager[num - 1];
    }
}
