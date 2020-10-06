using System;
using UnityEngine;

public class Type3Test : MonoBehaviour
{
    public void Testadder()
    {
        Debug.Log("Clicked.");
        Type3Manager.craneStatus++;
    }

    public void TestSubber()
    {
        Debug.Log("Clicked.");
        Type3Manager.craneStatus--;
    }

}