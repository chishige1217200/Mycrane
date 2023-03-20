using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CraneBoxSound : MonoBehaviour
{
    // [SerializeField] CraneBox craneBox;
    [SerializeField] SEPlayer sp; // 0:Start, 1:Moving, 2:Stop
    [SerializeField] bool useModule = false;
    Coroutine sound;

    void Start()
    {
        // craneBox = GetComponent<CraneBox>();
        sp = GetComponent<SEPlayer>();
    }

    public void MoveSound(bool flag)
    {
        if (useModule)
        {
            if (flag)
            {
                sp.Play(0, 1);
                sound = StartCoroutine(MovingUpdate());
            }
            else
            {
                if (sound != null) StopCoroutine(sound);
                sp.Stop(1);
                sp.Play(2, 1);
                sound = null;
            }
        }
    }

    private IEnumerator MovingUpdate()
    {
        while (true)
        {
            if (!sp.audioSource[0].isPlaying)
            {
                sp.Play(1);
                yield break;
            }
            yield return null;
        }
    }
}
