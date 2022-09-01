using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type13Player : MonoBehaviour
{
    public int craneStatus
    {
        get { return _craneStatus; }

        set {; }
    } //クレーン状態
    private int _craneStatus = 0;
    protected bool probability; //確率判定用
    ProbabilitySystem probabilitySystem;
    [SerializeField] float[] armPowerConfig = new float[3]; //アームパワー(%，未確率時)
    [SerializeField] float[] armPowerConfigSuccess = new float[3]; //アームパワー(%，確率時)
    [SerializeField] int limitTimeSet = 60; //残り時間を設定
    public bool leverTilted = false; //trueならレバーがアクティブ
    [SerializeField] bool downStop = true; //下降停止の利用可否
    [SerializeField] int downTime = 0; //0より大きく4600以下のとき有効，下降時間設定
    public float armPower; //現在のアームパワー
    public CraneBox craneBox;
    Type8ArmController armController;
    RopeManager ropeManager;
    Lever lever;
    Timer timer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
