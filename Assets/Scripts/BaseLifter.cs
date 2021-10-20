using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLifter : MonoBehaviour
{
    public abstract void Down();
    public abstract void DownForceStop();
    public abstract void Up();
    public abstract void UpForceStop();
    public abstract bool DownFinished();
    public abstract bool UpFinished();
}
