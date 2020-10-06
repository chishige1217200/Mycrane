using System;
using UnityEngine;

public class Type2Test : MonoBehaviour
{
    public void Testadder()
    {
        Debug.Log("Clicked.");
        Type2Manager.craneStatus++;
    }

    public void TestSubber()
    {
        Debug.Log("Clicked.");
        Type2Manager.craneStatus--;
    }

}