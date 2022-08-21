using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraneStatusViewer : MonoBehaviour
{
    [SerializeField] Text[] view;
    [SerializeField] CraneManager[] src;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < src.Length; i++)
        {
            view[i].text = src[i].GetStatus().ToString();
        }
    }
}
