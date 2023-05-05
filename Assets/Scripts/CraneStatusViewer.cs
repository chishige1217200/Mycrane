using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraneStatusViewer : MonoBehaviour
{
    [SerializeField] Text[] view;
    [SerializeField] CraneManagerV3[] src;

    void Update()
    {
        for (int i = 0; i < src.Length; i++)
        {
            view[i].text = src[i].craneStatus.ToString();
        }
    }

    public void AddStatus()
    {
        for (int i = 0; i < src.Length; i++)
        {
            src[i].craneStatus += 1;
        }
    }
}
