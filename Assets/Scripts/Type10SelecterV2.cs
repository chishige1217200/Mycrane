using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type10SelecterV2 : MonoBehaviour
{
    [SerializeField] Type10ManagerV2[] manager = new Type10ManagerV2[2];
    BGMPlayer bp;
    [SerializeField] int operationType = 0;
    [SerializeField] bool doAutoStart = true;
    private IEnumerator DelayCoroutine(float miliseconds, Action action)
    {
        yield return new WaitForSeconds(miliseconds / 1000f);
        action?.Invoke();
    }
    void Start()
    {
        bp = transform.Find("BGM").GetComponent<BGMPlayer>();
        manager[0].operationType = operationType;
        manager[1].operationType = operationType;
        if (doAutoStart)
        {
            StartCoroutine(DelayCoroutine(10, () =>
            {
                manager[0].Init();
                manager[1].Init();
            }));
            // ここに自動スタート処理
        }
    }

    void Update()
    {
        if (manager[0].craneStatus > 5 || manager[1].craneStatus > 5)
        {
            bp.Stop(0);
            bp.Stop(1);
            bp.Stop(2);
        }
        else if (manager[0].craneStatus > 0 || manager[1].craneStatus > 0)
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
        else if (manager[0].craneStatus == 0 && manager[1].craneStatus == 0)
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
}
