using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmUnitLifterV3Tester : MonoBehaviour
{
    [SerializeField] ArmUnitLifterV3 auf;
    [SerializeField] float targetHeight = -0.26f;

    void Start()
    {
        if (auf == null) Debug.LogError("テスト対象のArmUnitLifterV3がありません");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            auf.Up(true);
        else if (Input.GetKeyUp(KeyCode.I))
            auf.Up(false);
        if (Input.GetKeyDown(KeyCode.K))
            auf.Down(true);
        else if (Input.GetKeyUp(KeyCode.K))
            auf.Down(false);

        if (Input.GetKeyDown(KeyCode.Comma))
            auf.GoPosition(targetHeight);
    }
}