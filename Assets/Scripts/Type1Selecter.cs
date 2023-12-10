using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type1Selecter : MonoBehaviour
{
    [SerializeField] Type1Manager[] manager = new Type1Manager[2];
    [SerializeField] int soundType = 0;
    BGMPlayer bp;

    void Start()
    {
        manager[0].soundType = soundType;
        manager[1].soundType = soundType;
        bp = this.transform.Find("BGM").GetComponent<BGMPlayer>();
    }

    void Update()
    {
        if (manager[0].GetStatus() >= 0 && manager[1].GetStatus() >= 0 && !bp.audioSources[soundType].isPlaying)
            bp.Play(soundType);
    }

    public Type1Manager GetManager(int num)
    {
        return manager[num - 1];
    }
}
