using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type10Selecter : MonoBehaviour
{
    [SerializeField] Type10Manager[] manager = new Type10Manager[2];
    BGMPlayer bp;
    [SerializeField] int operationType = 0;

    void Start()
    {
        bp = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        manager[0].operationType = operationType;
        manager[1].operationType = operationType;
    }

    void Update()
    {
        if (manager[0].GetStatus() > 5 || manager[1].GetStatus() > 5)
        {
            bp.Stop(0);
            bp.Stop(1);
            bp.Stop(2);
        }
        else if (manager[0].GetStatus() > 0 || manager[1].GetStatus() > 0)
        {
            switch (operationType)
            {
                case 0:
                    bp.Stop(1);
                    bp.Play(2);
                    break;
                case 1:
                    bp.Stop(0);
                    bp.Play(2);
                    break;
            }
        }
        else if (manager[0].GetStatus() == 0 && manager[1].GetStatus() == 0)
        {
            switch (operationType)
            {
                case 0:
                    bp.Play(1);
                    break;
                case 1:
                    bp.Play(0);
                    break;
            }
        }
    }

    public Type10Manager GetManager(int num)
    {
        return manager[num - 1];
    }
}
