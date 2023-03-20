using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] GameObject[] vertices = new GameObject[2];
    [SerializeField] bool[] useMeshRenderer = new bool[2];
    LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        //line.material = new Material(Shader.Find("Unlit/Color"));
        line.positionCount = vertices.Length;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (useMeshRenderer[i] == false)
                vertices[i].GetComponent<MeshRenderer>().enabled = false;
        }

        // foreach (GameObject v in vertices)
        // {
        //     v.GetComponent<MeshRenderer>().enabled = false;
        // }
    }

    void Update()
    {
        int idx = 0;
        foreach (GameObject v in vertices)
        {
            line.SetPosition(idx, v.transform.position);
            idx++;
        }
    }
}
