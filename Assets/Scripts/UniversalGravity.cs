using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalGravity : MonoBehaviour {
    Rigidbody rb;
    float CONST_G = 0.00001f; //0.0001fがデフォルト
    public string TAG_NAME = "NULL";
    // Start is called before the first frame update
    void Start () {
        //gameObject.AddComponent<Rigidbody>(); // *既に加えられているRigidbodyコンポーネントを使用
        rb = gameObject.GetComponent<Rigidbody> ();
        rb.useGravity = false;

        gameObject.tag = TAG_NAME;
    }

    // Update is called once per frame
    void Update () {
        var objs = GameObject.FindGameObjectsWithTag (TAG_NAME);

        var p0 = rb.position;

        foreach (var obj in objs) {
            var trb = obj.GetComponent<Rigidbody> ();
            var q0 = trb.position;
            var pmq = p0 - q0;

            var F = -CONST_G * rb.mass * trb.mass * pmq * Mathf.Pow (pmq.magnitude, 3);
            rb.AddForce (F, ForceMode.Impulse);
        }
    }
}