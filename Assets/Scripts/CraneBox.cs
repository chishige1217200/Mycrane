using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneBox : MonoBehaviour
{
    public bool rightMoveFlag = false;
    public bool leftMoveFlag = false;
    public bool backMoveFlag = false;
    public bool forwardMoveFlag = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "LeftLimit")
        {
            Debug.Log("Hit");
            leftMoveFlag = false;
        }

        if (collider.tag == "RightLimit") rightMoveFlag = false;
        if (collider.tag == "BackgroundLimit") backMoveFlag = false;
        if (collider.tag == "ForegroundLimit") forwardMoveFlag = false;
    }
}
