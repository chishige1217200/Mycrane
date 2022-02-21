using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnProbability : MonoBehaviour
{
    public int creditProbability = 3;
    int timesTargeted = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (creditProbability <= 1) Destroy(this.gameObject);
    }

    public void IncrimentTarget()
    {
        timesTargeted++;
        if (creditProbability - 1 == timesTargeted) Destroy(this.gameObject);
    }
}
