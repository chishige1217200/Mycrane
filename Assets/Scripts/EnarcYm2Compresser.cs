using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnarcYm2Compresser : MonoBehaviour
{
    [SerializeField] int resetCount = 10; // コンプレッサー作動音を鳴らす周期
    private int remainingCount = 1;
    [SerializeField] SEPlayer sp;
    private IEnumerator DelayCoroutine(float miliseconds, Action action)
    {
        yield return new WaitForSeconds(miliseconds / 1000f);
        action?.Invoke();
    }
    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SEPlayer>();
    }

    public void DoCompressor()
    {
        remainingCount--;
        if (remainingCount == 0)
        {
            remainingCount = resetCount;
            StartCoroutine(DelayCoroutine(1000, () =>
            {
                sp.Play(0, 1);
            }));
        }
    }
}
