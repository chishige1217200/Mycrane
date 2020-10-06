using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    public void Testadder()
    {
        Debug.Log("Clicked.");
        Type1Manager.craneStatus++;
    }

    public void TestSubber()
    {
        Debug.Log("Clicked.");
        Type1Manager.craneStatus--;
    }

}