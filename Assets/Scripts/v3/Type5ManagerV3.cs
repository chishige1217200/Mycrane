using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type5ManagerV3 : CraneManagerV3
{
    public Type5ManagerConfig config; // null出ない場合は自動でsetup()を行う
    public bool player2 = false; // player2の場合true
    [SerializeField] TextMesh credit3d;
    [SerializeField] TextMesh[] preset = new TextMesh[4];
    [HideInInspector] public Animator[] animator = new Animator[3];
    [HideInInspector] public Type5NetworkV3 net;
    [HideInInspector] public int soundType = 0;
    [HideInInspector] public bool buttonPushed = false; // trueならボタンをクリックしているかキーボードを押下している
    [HideInInspector] public float armLPower;
    [HideInInspector] public float armRPower;
    private TwinArmController armController;
    private ArmControllerSupportV3 support;
    private ArmUnitLifterV3 lifter;
    private TimerV3 timer;
    private ArmNailV3[] nail = new ArmNailV3[2];

    // Start is called before the first frame update
    void Start()
    {
        Transform temp = transform.Find("CraneUnit").transform; ;

        craneStatus = -1;
        craneType = 5;
        host.manualCode = 7;
        // 様々なコンポーネントの取得
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystemV3>();
        // sp = transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPointV3>();
        timer = transform.Find("Timer").GetComponent<TimerV3>();
        lifter = temp.Find("CraneBox").Find("Tube").Find("TubePoint").GetComponent<ArmUnitLifterV3>();
        armController = temp.Find("ArmUnit").GetComponent<TwinArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupportV3>();
        craneBox = temp.Find("CraneBox").GetComponent<CraneBoxV3>();
        creditSystem.SetSEPlayer(sp);
        getPoint.SetManager(this);
        creditSystem.SetSEPlayer(sp);
        support.SetManager(this);
        support.SetLifter(lifter);
        lifter.Up(true);

        // if (!player2)
        // {
        //     config.startPoint = new Vector2(-0.61f + config.startPoint.x, -0.31f + config.startPoint.y);
        //     config.homePoint = new Vector2(-0.61f + config.homePoint.x, -0.31f + config.homePoint.y);
        //     if (config.boxRestrictions[0] < 100) xLimit.localPosition = new Vector3(-0.5f + 0.004525f * config.boxRestrictions[0], xLimit.localPosition.y, xLimit.localPosition.z);
        // }
        // else
        // {
        //     config.startPoint = new Vector2(0.61f - config.startPoint.x, -0.31f + config.startPoint.y);
        //     config.homePoint = new Vector2(0.61f - config.homePoint.x, -0.31f + config.homePoint.y);
        //     if (config.boxRestrictions[0] < 100) xLimit.localPosition = new Vector3(0.5f - 0.004525f * config.boxRestrictions[0], xLimit.localPosition.y, xLimit.localPosition.z);
        // }
        // if (config.boxRestrictions[1] < 100) zLimit.localPosition = new Vector3(zLimit.localPosition.x, zLimit.localPosition.y, -0.19f + 0.00605f * config.boxRestrictions[1]);
        // if (config.downRestriction < 100) downLimit.localPosition = new Vector3(downLimit.localPosition.x, 1.4f - 0.005975f * config.downRestriction, downLimit.localPosition.z);
        // craneBox.GoPosition(config.startPoint);

        if (config != null)
        {
            StartCoroutine(DelayCoroutine(1000, () => { Setup(config); }));
        }
    }

    // Setup実行前に他のコンポーネントの処理を済ませること(ArmUnitLifterV3, CraneBoxV3, CreditSystemV3, ProbabilitySystemV3)
    public void Setup(Type5ManagerConfig config)
    {
        this.config = config;

        Transform temp = transform.Find("CraneUnit").transform; ;

        if (isHibernate)
        {
            creditSystem.SetHibernate();
        }

        if (!isHibernate)
        {
            if (!transform.parent.Find("LCD").gameObject.activeSelf)
                transform.parent.Find("LCD").gameObject.SetActive(true);
            preset[0].text = creditSystem.priceSets[0].ToString();
            preset[1].text = creditSystem.priceSets[1].ToString();
            preset[2].text = creditSystem.timesSets[0].ToString();
            preset[3].text = creditSystem.timesSets[1].ToString();
            if (!player2)
            {
                transform.parent.Find("LCD Component").Find("SegUnit3").gameObject.SetActive(true);
                transform.parent.Find("LCD Component").Find("SegUnit1").gameObject.SetActive(true);
            }
            else
            {
                transform.parent.Find("LCD Component").Find("SegUnit3 (1)").gameObject.SetActive(true);
                transform.parent.Find("LCD Component").Find("SegUnit1 (1)").gameObject.SetActive(true);
            }
        }

        if (!(isHibernate | creditSystem.priceSets[1] == 0 || creditSystem.timesSets[1] == 0 ||
(float)creditSystem.priceSets[0] / creditSystem.timesSets[0] < (float)creditSystem.priceSets[1] / creditSystem.timesSets[1]))
        // 正常なクレジットサービスが可能なとき
        {
            if (!player2)
                transform.parent.Find("LCD Component").Find("SegUnit2").gameObject.SetActive(true);
            else
                transform.parent.Find("LCD Component").Find("SegUnit2 (1)").gameObject.SetActive(true);
        }

        if (!config.downStop)
        {
            transform.Find("Canvas").Find("ControlGroup").Find("Button 3").gameObject.SetActive(false);
            transform.Find("Floor").Find("Button3").gameObject.SetActive(false);
        }

        for (int i = 0; i < 2; i++)
        {
            string a = "Arm" + (i + 1).ToString();
            GameObject arm;
            switch (config.armSize[i])
            {
                case 0:
                case 1:
                    a += "S";
                    break;
                case 2:
                    a += "M";
                    break;
                case 3:
                    a += "L";
                    break;
            }
            arm = temp.Find("ArmUnit").Find(a).gameObject;
            nail[i] = arm.transform.Find("Nail").GetComponent<ArmNailV3>();
            if (config.armSize[i] != 0) arm.SetActive(true);
            armController.SetArm(i, config.armSize[i]);
        }

        for (int i = 0; i < 2; i++)
        {
            nail[i].SetManager(this);
            nail[i].SetLifter(lifter);
        }

        if (soundType == 0 || soundType == 1 || soundType == 2) creditSystem.SetSoundNum(0);
        else creditSystem.SetSoundNum(8);

        switch (soundType)
        {
            case 0:
            case 1:
            case 2:
                getSoundNum = 7;
                break;
            case 3:
                getSoundNum = 15;
                break;
        }

        support.pushTime = config.pushTime; // 押し込みパワーの調整
        armController.SetLimit(config.armApertures);
        // craneStatus = -2;
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        // ロープ巻取り確認
        if (!lifter.CheckPos(1))
        {
            lifter.Up(true);
            while (!lifter.CheckPos(1))
            {
                yield return null;
            }
        }

        // アームユニット位置確認
        if (!player2 && !craneBox.CheckPos(1))
        {
            craneBox.Left(true);
            craneBox.Forward(true);
            while (!craneBox.CheckPos(1))
            {
                yield return null;
            }
        }
        else if (player2 && !craneBox.CheckPos(3))
        {
            craneBox.Right(true);
            craneBox.Forward(true);
            while (!craneBox.CheckPos(3))
            {
                yield return null;
            }
        }

        craneBox.cbs.useModule = true;

        // 初期位置及び高さへ遷移

        creditSystem.Setup();
        craneStatus = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (useUI && host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if (!player2 && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();
        else if (player2 && (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus))) InsertCoin();
    }

    protected override void DetectKey(int num) { }
    public override void ButtonDown(int num) { }
    public override void ButtonUp(int num) { }
    public override void InsertCoin()
    {
        if (!isHibernate && host.playable && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 100) credit3d.text = credit.ToString();
            else credit3d.text = "99.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }

    public override void InsertCoinAuto()
    {
        if (!isHibernate && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 100) credit3d.text = credit.ToString();
            else credit3d.text = "99.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }
    protected override void FirstStatusEvent(int status) { }
    protected override void LastStatusEvent(int status) { }
}
