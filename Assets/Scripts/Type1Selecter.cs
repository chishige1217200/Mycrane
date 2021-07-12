using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type1Selecter : MonoBehaviour
{
    [SerializeField] Type1Manager[] manager = new Type1Manager[2];
    [SerializeField] int soundType = 0;

    void Start()
    {
        manager[0].soundType = soundType;
        manager[1].soundType = soundType;
    }

    public Type1Manager GetManager(int num)
    {
        return manager[num - 1];
    }
}
