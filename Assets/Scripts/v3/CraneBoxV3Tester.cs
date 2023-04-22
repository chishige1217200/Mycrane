using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneBoxV3Tester : MonoBehaviour
{
    [SerializeField] CraneBoxV3 cb;
    [SerializeField] Vector2 goPoint;

    void Start()
    {
        if (cb == null) Debug.LogError("テスト対象のCraneBoxV3がありません");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            cb.Right(true);
        else if (Input.GetKeyUp(KeyCode.H))
            cb.Right(false);
        if (Input.GetKeyDown(KeyCode.F))
            cb.Left(true);
        else if (Input.GetKeyUp(KeyCode.F))
            cb.Left(false);
        if (Input.GetKeyDown(KeyCode.T))
            cb.Back(true);
        else if (Input.GetKeyUp(KeyCode.T))
            cb.Back(false);
        if (Input.GetKeyDown(KeyCode.G))
            cb.Forward(true);
        else if (Input.GetKeyUp(KeyCode.G))
            cb.Forward(false);

        if (Input.GetKeyDown(KeyCode.V))
            cb.GoPosition(goPoint);
    }

}
