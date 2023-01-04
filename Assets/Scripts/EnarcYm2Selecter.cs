using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnarcYm2Selecter : MonoBehaviour
{
    [SerializeField] EnarcYm2Manager[] manager = new EnarcYm2Manager[8];
    BGMPlayer bp;
    [SerializeField] int soundType = 0; //0：エラー，1：BGM
    [SerializeField] bool doRotate = false;
    // Start is called before the first frame update
    void Start()
    {
        bp = transform.Find("BGM").GetComponent<BGMPlayer>();
        if (soundType == 0) bp.Play(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlaying())
        {
            if (soundType == 1)
            {
                if (bp.audioSource[1].isPlaying) bp.Stop(1);
                bp.Play(2);
            }
        }
        else
        {
            if (soundType == 1)
            {
                if (bp.audioSource[2].isPlaying) bp.Stop(2);
                bp.Play(1);
            }
        }
    }

    void FixedUpdate()
    {
        if(doRotate)
        {
            transform.localEulerAngles += new Vector3(0, -0.05f, 0);
        }
    }

    bool IsPlaying()
    {
        int checker = 0; // プレイ中のブース数

        for (int i = 0; i < manager.Length; i++)
        {
            if (manager[i] != null && manager[i].craneStatus > 0)
                checker++;
        }

        if (checker > 0) return true;
        return false;
    }
}
