using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerV3Tester : MonoBehaviour
{
    [SerializeField] TimerV3 timer;
    [SerializeField] int limitTime = 10;
    [SerializeField] Text seg;
    void Start()
    {
        if (timer == null) Debug.LogError("テスト対象のTimerV3がありません");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            timer.Activate(limitTime);
        if (Input.GetKeyDown(KeyCode.J))
            timer.Cancel();

        seg.text = timer.GetLimitTimeNow().ToString("D2");
    }
}
