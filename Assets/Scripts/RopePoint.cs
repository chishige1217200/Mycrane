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
        await Task.Delay(3000);
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
            this.transform.position = new Vector3(0, this.transform.position.y, 0);
            this.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
        else if (collider.tag == "UpPoint")
        {
            if (!parent)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
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
        if (collider.tag == "DownStopPoint")
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
        {
            this.transform.position += new Vector3(0, 0.2f, 0);
            if (this.transform.position.x < -0.5f)
                rb.AddForce(new Vector3(0.001f, 0, 0), ForceMode.Impulse);
            if (this.transform.position.x > 0.5f)
                rb.AddForce(new Vector3(-0.001f, 0, 0), ForceMode.Impulse);
            if (this.transform.position.z < -0.5f)
                rb.AddForce(new Vector3(0, 0, 0.001f), ForceMode.Impulse);
            if (this.transform.position.x > 0.5f)
                rb.AddForce(new Vector3(0, 0, -0.001f), ForceMode.Impulse);
        }
    }

    void RopeDown()
    {
        if (moveFlag)
            this.transform.position -= new Vector3(0, 0.2f, 0);
    }
}
