using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type1Selecter : MonoBehaviour
{
    [SerializeField] Type1Manager[] manager = new Type1Manager[2];

    public Type1Manager GetManager(int num)
    {
        return manager[num - 1];
    }
}
