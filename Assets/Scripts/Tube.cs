using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour
{
    [SerializeField] GameObject tube;
    GameObject craneUnit;
    public bool parent = false; //一番上の質点かどうか
    public bool last = false; //下から二番目の質点かどうか

    void Start()
    {
        craneUnit = this.transform.parent.parent.parent.Find("ArmUnit").Find("Main").Find("TubePoint").gameObject;
    }

    void FixedUpdate()
    {
        if (craneUnit.transform.position.y > tube.transform.position.y - tube.transform.lossyScale.y * 2.0f)
            this.transform.position = new Vector3(this.transform.position.x, craneUnit.transform.position.y, this.transform.position.z);
    }
}
