using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CraneBoxSound : MonoBehaviour
{
    [SerializeField] SEPlayer sp; // 0:Start, 1:Moving, 2:Stop
    [SerializeField] bool useModule = false;
    private bool soundPlaying = false;
    private Coroutine sound;

    void Start()
    {
        // craneBox = GetComponent<CraneBox>();
        sp = GetComponent<SEPlayer>();
    }

    public void MoveSound(bool flag)
    {
        if (useModule && flag != soundPlaying)
        {
            if (flag)
            {
                soundPlaying = true;
                sp.ForcePlay(0);
                sound = StartCoroutine(MovingUpdate());
            }
            else
            {
                soundPlaying = false;
                if (sound != null) StopCoroutine(sound);
                sp.Stop(0);
                sp.Stop(1);
                sp.Play(2, 1);
                sound = null;
            }
        }
    }

    private IEnumerator MovingUpdate()
    {
        sp.Stop(2);
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
