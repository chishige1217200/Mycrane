using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinArmControllerTester : MonoBehaviour
{
    [SerializeField] TwinArmController tac;
    // Start is called before the first frame update
    void Start()
    {
        if (tac == null) Debug.LogError("テスト対象のTwinArmControllerがありません");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            tac.Open();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            tac.Close(30);
        }
    }
}
