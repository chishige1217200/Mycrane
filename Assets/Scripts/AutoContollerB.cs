using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoContollerB : MonoBehaviour
{
    [SerializeField] CraneManager target;
    public MachineHost host;
    bool autoPlay
    {
        get
        { return autoPlay; }
        set
        {
            if (value != _autoPlay)
            {
                _autoPlay = value;
                if (_autoPlay && c == null)
                {
                    Operate();
                }
                else
                {
                    StopCoroutine(c);
                    c = null;
                }
            };
        }
    }
    Coroutine c;
    bool _autoPlay = false;
    // 要素が1つのときは固定値，2つのときはその間のランダム時間で処理を行う．
    [SerializeField] float[] coinInterval = new float[2];
    [SerializeField] float[] playInterval = new float[2];
    [SerializeField] float[] button1Interval = new float[2];
    [SerializeField] float[] button2Interval = new float[2];
    [SerializeField] bool button3 = false;
    [SerializeField] float[] button3Interval = new float[2];

    private IEnumerator DelayCoroutine(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }

    void Start()
    {
        if (host == null)
            Debug.LogError("hostが設定されていません");
        if (target == null)
            Debug.LogError("targetが設定されていません");
    }

    void Update()
    {
        if (AutoControllerFlag.doAutoPlay)
            autoPlay = !host.playable;
        else
            autoPlay = false;
    }

    void Operate()
    {
        c = StartCoroutine("AutoOperate");
    }

    private IEnumerator AutoOperate()
    {
        yield return new WaitForSeconds(10f);
        Debug.Log("Start AutoControl");
        target.InsertCoinAuto();
        for (int i = 1; i < target.priceSet[0] / 100; i++)
        {
            if (coinInterval.Length == 1)
                yield return new WaitForSeconds(coinInterval[0]);
            else if (coinInterval.Length == 2)
            {
                float time = UnityEngine.Random.Range(coinInterval[0], coinInterval[1]);
                Debug.Log(time);
                yield return new WaitForSeconds(time);
            }
            target.InsertCoinAuto();
        }

        yield return new WaitForSeconds(3f);

        target.ButtonDown(1);
        if (button1Interval.Length == 1)
            yield return new WaitForSeconds(button1Interval[0]);
        else if (button1Interval.Length == 2)
        {
            float time = UnityEngine.Random.Range(button1Interval[0], button1Interval[1]);
            Debug.Log(time);
            yield return new WaitForSeconds(time);
        }
        target.ButtonUp(1);

        yield return new WaitForSeconds(3f);

        target.ButtonDown(2);
        if (button2Interval.Length == 1)
            yield return new WaitForSeconds(button2Interval[0]);
        else if (button2Interval.Length == 2)
        {
            float time = UnityEngine.Random.Range(button2Interval[0], button2Interval[1]);
            Debug.Log(time);
            yield return new WaitForSeconds(time);
        }
        target.ButtonUp(2);

        if (button3)
        {
            yield return new WaitForSeconds(3f);
            target.ButtonDown(3);
            if (button3Interval.Length == 1)
                yield return new WaitForSeconds(button3Interval[0]);
            else if (button3Interval.Length == 2)
            {
                float time = UnityEngine.Random.Range(button3Interval[0], button3Interval[1]);
                Debug.Log(time);
                yield return new WaitForSeconds(time);
            }
            target.ButtonUp(3);
        }

        if (playInterval.Length == 1)
            yield return new WaitForSeconds(playInterval[0]);
        else if (playInterval.Length == 2)
        {
            float time = UnityEngine.Random.Range(playInterval[0], playInterval[1]);
            Debug.Log(time);
            yield return new WaitForSeconds(time);
        }
        c = StartCoroutine("AutoOperate");
    }
}
