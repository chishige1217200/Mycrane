using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {
    public GameObject[] vertices = new GameObject[5];
    LineRenderer line;

    void Start () {
        line = GetComponent<LineRenderer> ();
        line.material = new Material (Shader.Find ("Unlit/Color"));
        line.positionCount = vertices.Length;

        foreach (GameObject v in vertices) {
            v.GetComponent<MeshRenderer> ().enabled = false;
        }
    }

    void Update () {
        int idx = 0;
        foreach (GameObject v in vertices) {
            line.SetPosition (idx, v.transform.position);
            idx++;
        }
    }
}