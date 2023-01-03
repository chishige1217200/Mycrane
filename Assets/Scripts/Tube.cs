using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour
{
    [SerializeField] GameObject tube;
    GameObject craneUnit;
    public bool parent = false; //一番上の質点かどうか
    public bool last = false; //下から二番目の質点かどうか
    float craneUnitdiff = 0f; // craneUnitオブジェクトとのy座標差

    void Start()
    {
        craneUnit = transform.parent.Find("TubePoint").gameObject;
        craneUnitdiff = Mathf.Abs(craneUnit.transform.position.y - transform.position.y);
    }

    void FixedUpdate()
    {
        if (!parent)
        {
            if (craneUnit.transform.position.y > tube.transform.position.y - (tube.transform.lossyScale.y + transform.lossyScale.y * 2.0f))
                transform.position = new Vector3(transform.position.x, craneUnit.transform.position.y + craneUnitdiff, transform.position.z);
        }
    }
}
