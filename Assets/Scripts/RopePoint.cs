using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RopePoint : MonoBehaviour
{
    public bool parent = false; //一番上の質点かどうか
    public bool last = false; //下から二番目の質点かどうか
    Rigidbody rb; //Rigidbody情報

    bool moveFlag = false; //上昇または下降をしているか

    async void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        await Task.Delay(1000);
        if (parent)
            moveFlag = true;
    }

    void Update()
    {
        RopeUp();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "UpLimit")
        {
            moveFlag = false;
        }
        else if (collider.tag == "UpPoint")
        {
            if (!parent)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
                moveFlag = true;
            }
        }
    }

    /*void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "UpLimit")
        {
            if (parent)
                moveFlag = false;
        }
    }*/

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "UpLimit")
        {
            if (parent)
            {
                if (moveFlag)
                    moveFlag = false;
            }
            else
            {
                if (moveFlag)
                {
                    rb.useGravity = true;
                    rb.isKinematic = false;
                    moveFlag = false;
                }
            }
        }
    }

    void RopeUp()
    {
        if (moveFlag)
            this.transform.position += new Vector3(0, 0.1f, 0);
    }

    void RopeDown()
    {
        if (moveFlag)
            this.transform.position -= new Vector3(0, 0.1f, 0);
    }

    void ResetRopePointPosition()
    {
        this.transform.position = new Vector3(0, 0, 0);
    }


}
