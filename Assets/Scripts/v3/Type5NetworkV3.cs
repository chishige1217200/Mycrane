using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CelebrationSync : MonoBehaviour // NetWorkに参加するSelecter相当が実装する
{
    public abstract void SetupNetWork(Type5NetworkV3 net);
    public abstract void Celebrate();
}

public class Type5NetworkV3 : MonoBehaviour
{
    [SerializeField] CelebrationSync[] machines;

    void Start()
    {
        for (int i = 0; i < machines.Length; i++)
        {
            machines[i].SetupNetWork(this);
        }
    }

    public void CelebrateAll()
    {
        for (int i = 0; i < machines.Length; i++)
        {
            machines[i].Celebrate();
        }
    }
}
