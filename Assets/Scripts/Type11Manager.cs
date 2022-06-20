using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type11Manager : CraneManager
{
    public int[] priceSet = new int[2];
    public int[] timesSet = new int[2];
    [SerializeField] int[] downTime = new int[2];
    bool[] isExecuted = new bool[15]; //各craneStatusで1度しか実行しない処理の管理
    public bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] TextMesh credit3d;
    [SerializeField] TextMesh[] preset = new TextMesh[4];
    // Start is called before the first frame update
    void Start()
    {
        craneStatus = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
    }

    protected override void DetectKey(int num)
    {
    }

    public override void ButtonDown(int num)
    {
    }

    public void ButtonUp(int num)
    {
    }

    public override void InsertCoin()
    {
    }
}
