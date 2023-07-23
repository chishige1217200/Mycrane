using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Type13Player : MonoBehaviour
{
    [SerializeField] int boothNumber = 0;
    public int craneStatus
    {
        get { return _craneStatus; }

        set
        {
            LastStatusEvent(_craneStatus);
            _craneStatus = value;
            FirstStatusEvent(_craneStatus);
        }
    } //クレーン状態
    private int _craneStatus = 0;
    protected bool probability; //確率判定用
    ProbabilitySystem probabilitySystem;
    Type13Manager manager;
    [SerializeField] float[] armPowerConfig = new float[2]; //アームパワー(%，未確率時)
    [SerializeField] float[] armPowerConfigSuccess = new float[2]; //アームパワー(%，確率時)
    [SerializeField] int limitTimeSet = 60; //残り時間を設定
    public bool leverTilted = false; //trueならレバーがアクティブ
    [SerializeField] bool downStop = true; //下降停止の利用可否
    [SerializeField] int downTime = 0; //0より大きく4600以下のとき有効，下降時間設定
    public float armPower; //現在のアームパワー
    public CraneBox craneBox;
    Type8ArmController armController;
    BaseLifter ropeManager;
    Lever lever;
    //Timer timer;
    TimerV3 timer;
    [SerializeField] Text limitTimedisplayed;
    [SerializeField] TextMesh limitTime3d;
    private IEnumerator DelayCoroutine(float miliseconds, Action action)
    {
        yield return new WaitForSeconds(miliseconds / 1000f);
        action?.Invoke();
    }
    // Start is called before the first frame update
    void Start()
    {
        Transform temp;

        craneStatus = -2;
        manager = transform.parent.GetComponent<Type13Manager>();
        probabilitySystem = GetComponent<ProbabilitySystem>();
        lever = transform.parent.Find("Canvas").Find("ControlGroup").Find("Lever 1").GetComponent<Lever>();
        timer = GetComponent<TimerV3>();
        // timer.SetSEPlayer(manager.sp);
        temp = transform.Find("CraneUnit").transform;
        //timer.SetSEPlayer(sp);

        // ロープとアームコントローラに関する処理
        ropeManager = transform.Find("RopeManager").GetComponent<BaseLifter>();
        armController = temp.Find("ArmUnit").GetComponent<Type8ArmController>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();

        StartCoroutine(DelayCoroutine(300, () =>
        {
            StartCoroutine(InternalStart());
        }));
    }

    IEnumerator InternalStart()
    {
        ropeManager.Up();
        while (!ropeManager.UpFinished())
        {
            yield return new WaitForSeconds(0.1f);
        }
        armController.Open();

        craneStatus = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (craneStatus == -1)
        {
            if (boothNumber == 0 || boothNumber == 3)
            {
                if (craneBox.CheckPos(1))
                    craneStatus = 0;
            }
            else
            {
                if (craneBox.CheckPos(3))
                    craneStatus = 0;
            }
        }
        if (craneStatus > 0)
        {
            if (Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.C) && !probability) probability = true; // テスト用隠しコマンド
            if (craneStatus == 1)
            {
                if (craneBox.CheckPos(9)) craneStatus = 2;
            }
            if (craneStatus == 3)
            {
                if (!leverTilted) DetectKey(craneStatus);
                if (timer.GetLimitTimeNow() <= 0) craneStatus = 4;
            }
            if (craneStatus == 4)
            {
                DetectKey(craneStatus);
                if (ropeManager.DownFinished()) craneStatus = 5;
            }
            if (craneStatus == 6)
            {
                if (ropeManager.UpFinished()) craneStatus = 7;
            }
            if (craneStatus == 8)
            {
                if (boothNumber < 2)
                {
                    if (craneBox.CheckPos(1))
                        craneStatus = 9;
                }
                else
                {
                    if (craneBox.CheckPos(3))
                        craneStatus = 9;
                }
            }
            if (craneStatus == 9)
            {
                if (boothNumber % 2 == 0 && ropeManager.DownFinished())
                    craneStatus = 10;
            }
            if (craneStatus == 12)
            {
                if (boothNumber % 2 == 0 && ropeManager.UpFinished())
                    craneStatus = 14;
            }
            if (craneStatus == 14)
            {
                if (boothNumber == 0 || boothNumber == 3)
                {
                    if (craneBox.CheckPos(1))
                    {
                        craneStatus = 0;
                        manager.PlayerEnd();
                    }
                }
                else
                {
                    if (craneBox.CheckPos(3))
                    {
                        craneStatus = 0;
                        manager.PlayerEnd();
                    }
                }
            }
            if (!manager.creditSystem.segUpdateFlag) // Timer表示用
            {
                if (timer.GetLimitTimeNow() >= 0)
                {
                    limitTimedisplayed.text = timer.GetLimitTimeNow().ToString("D2");
                    limitTime3d.text = timer.GetLimitTimeNow().ToString("D1");
                }
                else
                {
                    limitTimedisplayed.text = "00";
                    limitTime3d.text = "0";
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (craneStatus == -1)
        {
            craneBox.Forward();
            if (boothNumber == 0 || boothNumber == 3)
                craneBox.Left();
            else
                craneBox.Right();

        }
        if (craneStatus == 2 || craneStatus == 3)
            DetectLever();

        if (craneStatus == 8)
        {
            craneBox.Forward();
            if (boothNumber < 2)
                craneBox.Left();
            else
                craneBox.Right();
        }

        if (craneStatus == 14)
        {
            if (boothNumber == 1)
                craneBox.Right();
            else if (boothNumber == 3)
                craneBox.Left();
        }
    }

    public void GetPrize()
    {
        switch (probabilitySystem.probabilityMode)
        {
            case 2:
            case 3:
                probabilitySystem.ResetCreditProbability();
                break;
            case 4:
            case 5:
                probabilitySystem.ResetCostProbability();
                break;
        }
    }

    public void GameStart()
    {
        craneStatus = 1;
    }

    void DetectKey(int num)
    {
        if (manager.host.playable)
        {
            switch (num)
            {
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && boothNumber < 2)
                        StartCoroutine(DelayCoroutine(300, () =>
                        {
                            if (craneStatus == 3) craneStatus = 4;
                        }));
                    if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && boothNumber >= 2)
                        StartCoroutine(DelayCoroutine(300, () =>
                        {
                            if (craneStatus == 3) craneStatus = 4;
                        }));
                    break;
                case 4:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && boothNumber < 2 && downStop)
                        craneStatus = 5;
                    if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && boothNumber >= 2 && downStop)
                        craneStatus = 5;
                    break;
            }
        }
    }

    void DetectLever()
    {
        if (manager.host.playable)
        {
            if (boothNumber < 2)
            {
                if (Input.GetKey(KeyCode.H) || lever.rightFlag)
                    craneBox.Right();
                if (Input.GetKey(KeyCode.F) || lever.leftFlag)
                    craneBox.Left();
                if (Input.GetKey(KeyCode.T) || lever.backFlag)
                    craneBox.Back();
                if (Input.GetKey(KeyCode.G) || lever.forwardFlag)
                    craneBox.Forward();

                if (Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.G)
                || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag)
                    if (craneStatus == 2)
                        craneStatus = 3;
            }
            else //2Pレバー
            {
                if (Input.GetKey(KeyCode.L) || lever.rightFlag)
                    craneBox.Right();
                if (Input.GetKey(KeyCode.J) || lever.leftFlag)
                    craneBox.Left();
                if (Input.GetKey(KeyCode.I) || lever.backFlag)
                    craneBox.Back();
                if (Input.GetKey(KeyCode.K) || lever.forwardFlag)
                    craneBox.Forward();

                if (Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K)
                || lever.rightFlag || lever.leftFlag || lever.backFlag || lever.forwardFlag)
                    if (craneStatus == 2)
                        craneStatus = 3;
            }
        }
    }

    public void ButtonDown(int num)
    {
        if (manager.host.playable)
        {
            switch (num)
            {
                case 3:
                    StartCoroutine(DelayCoroutine(300, () =>
                    {
                        if (craneStatus == 3) craneStatus = 4;
                    }));
                    break;
                case 4:
                    if (downStop && craneStatus == 4)
                        craneStatus = 5;
                    break;
            }
        }
    }

    void FirstStatusEvent(int status)
    {
        if (status == 1)
        {
            if (boothNumber < 2) craneBox.goPoint = new Vector2(-0.24f, 0);
            else craneBox.goPoint = new Vector2(0.24f, 0);

            limitTime3d.text = limitTimeSet.ToString();
            craneBox.goPositionFlag = true;
        }
        else if (status == 3)
        {
            manager.creditSystem.segUpdateFlag = false;
            timer.Activate(limitTimeSet);
            probabilitySystem.NewPlay();
            probability = probabilitySystem.ProbabilityCheck();
            Debug.Log("Probability:" + probability);
        }
        else if (status == 4)
        {
            timer.Cancel();
            limitTime3d.text = "0";
            manager.creditSystem.segUpdateFlag = true;
            manager.ResetPayment();
            manager.PlayerStart();
            manager.CreditSegUpdate();
            ropeManager.Down();
            // 下降音
            manager.sp.Play(3);
            if (downTime > 0 && downTime <= 4600)
            {
                StartCoroutine(DelayCoroutine(downTime, () =>
                {
                    if (craneStatus == 4) craneStatus = 5;
                }));
            }
        }
        else if (status == 5)
        {
            //掴む音
            manager.sp.Stop(3);
            manager.sp.Play(4, 1);
            ropeManager.DownForceStop();
            armController.Close();
            StartCoroutine(DelayCoroutine(1000, () =>
            {
                if (probability)
                {
                    if (armPowerConfigSuccess[0] >= 30) armPower = armPowerConfigSuccess[0];
                    else armPower = 30f;
                }
                else
                {
                    if (armPowerConfig[0] >= 30) armPower = armPowerConfig[0];
                    else armPower = 30f;
                }
                armController.SetMotorPower(armPower);
                StartCoroutine(DelayCoroutine(2000, () =>
                {
                    if (craneStatus == 5) craneStatus = 6;
                }));
            }));

        }
        if (status == 6)
        {
            manager.sp.Play(5);
            ropeManager.Up();
        }
        if (status == 7)
        {
            manager.sp.Stop(5);
            if (probability) armPower = armPowerConfigSuccess[1];
            else armPower = armPowerConfig[1];
            armController.SetMotorPower(armPower);
            StartCoroutine(DelayCoroutine(3000, () =>
                {
                    if (craneStatus == 7) craneStatus = 8;
                }));
        }
        if (status == 9)
        {
            if (boothNumber % 2 == 0)
                ropeManager.Down();
            else
                craneStatus = 11;
        }
        if (status == 10)
        {
            StartCoroutine(DelayCoroutine(500, () =>
            {
                ropeManager.Up();
                StartCoroutine(DelayCoroutine(500, () =>
                {
                    ropeManager.UpForceStop();
                    StartCoroutine(DelayCoroutine(500, () =>
                    {
                        if (craneStatus == 10) craneStatus = 11;
                    }));
                }));
            }));
        }
        if (status == 11)
        {
            StartCoroutine(DelayCoroutine(500, () =>
            {
                armController.Open();
                StartCoroutine(DelayCoroutine(2000, () =>
                {
                    if (boothNumber % 2 == 0)
                        craneStatus = 12;
                    else
                        craneStatus = 14;
                }));
            }));
        }
        if (status == 12)
        {
            ropeManager.Up();
        }
    }

    void LastStatusEvent(int status)
    {
        if (status == -1)
            manager.IncrimentStatus();
    }
}
