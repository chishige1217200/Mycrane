using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour
{
    [SerializeField] GameObject tube;
    GameObject craneUnit;

    // Start is called before the first frame update
    void Start()
    {
        craneUnit = this.transform.parent.parent.parent.Find("ArmUnit").Find("Main").Find("TubePoint").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (craneUnit.transform.position.y > tube.transform.position.y - tube.transform.lossyScale.y * 2.0f)
        {
            this.transform.position = new Vector3(this.transform.position.x, craneUnit.transform.position.y, this.transform.position.z);
        }
    }
}
