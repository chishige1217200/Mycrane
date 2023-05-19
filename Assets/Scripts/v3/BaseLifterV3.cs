using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLifterV3 : MonoBehaviour
{
    public abstract void GoPosition(float height);
    public abstract void CancelGoPosition();
    public abstract void Up(bool flag);
    public abstract void Down(bool flag);
    public abstract bool CheckPos(int mode); // 1:上，2：下，3：GoPosition用
}
