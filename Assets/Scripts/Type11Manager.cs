using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Type11Manager : CraneManager
{
    public int[] priceSet = new int[2];
    public int[] timesSet = new int[2];
    [SerializeField] int[] downTime = new int[2];
    public bool isBonus { get; private set; } = false;
    bool[] isExecuted = new bool[17]; //各craneStatusで1度しか実行しない処理の管理
    public bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] bool player2 = false; //player2の場合true
    float moveSpeedBackup = 0f;
    Type11Stretcher s;
    [SerializeField] TextMesh credit3d;
    [SerializeField] TextMesh[] preset = new TextMesh[4];
    // Start is called before the first frame update
    async void Start()
    {
        Transform temp;
        craneStatus = -2;
        craneType = 11;

        // 様々なコンポーネントの取得
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystem>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = transform.Find("CraneUnit").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];
        if (isHibernate)
        {
            // credit3d.text = "-";
            creditSystem.SetHibernate();
            // preset[0].text = "---";
            // preset[1].text = "---";
            // preset[2].text = "-";
            // preset[3].text = "-";
        }
        else
        {
            // preset[0].text = priceSet[0].ToString();
            // preset[1].text = priceSet[1].ToString();
            // preset[2].text = timesSet[0].ToString();
            // preset[3].text = timesSet[1].ToString();
        }

        await Task.Delay(500);

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        s = temp.Find("CraneBox").Find("Nail").GetComponent<Type11Stretcher>();
        moveSpeedBackup = craneBox.moveSpeed;

        // ロープにマネージャー情報をセット
        creditSystem.SetSEPlayer(sp);
        getPoint.SetManager(this);
        creditSystem.SetCreditSound(0);

        for (int i = 0; i < isExecuted.Length; i++)
            isExecuted[i] = false;

        craneStatus = -1;
    }

    // Update is called once per frame
    async void Update()
    {
        if (host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if (!player2 && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();
        else if (player2 && (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus))) InsertCoin();

        if (craneStatus == -1 && ((craneBox.CheckPos(1) && !player2) || (craneBox.CheckPos(3) && player2))) craneStatus = 0;
        if (craneStatus > 0)
        {
            if (craneStatus == 1)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    int rand = Random.Range(1, 3);
                    if (rand == 2) isBonus = true;
                    else isBonus = false;

                    if (isBonus)
                    {
                        sp.Stop(0);
                        sp.Play(1, 1);
                    }
                }
                DetectKey(craneStatus);
            }

            if (craneStatus == 2)
            {
                // 上移動中
                sp.Play(2);
                DetectKey(craneStatus);
                if (craneBox.CheckPos(8))
                {
                    buttonPushed = false;
                    craneStatus = 3;
                }
            }

            if (craneStatus == 3)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    sp.Stop(2);
                    sp.Play(3, 1);
                }
                DetectKey(craneStatus);
            }

            if (craneStatus == 4)
            {
                sp.Play(4);
                DetectKey(craneStatus);
                if (!player2 && craneBox.CheckPos(7))
                {
                    buttonPushed = false;
                    craneStatus = 6;
                }
                if (player2 && craneBox.CheckPos(5))
                {
                    buttonPushed = false;
                    craneStatus = 6;
                }
            }

            if (craneStatus == 5)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    int slipTime = Random.Range(50, 201);
                    if (!probability)
                    {
                        Debug.Log("Machine will slip! : " + slipTime + "ms");
                        await Task.Delay(slipTime);
                    }
                    if (craneStatus == 5) craneStatus = 6;
                }

            }

            if (craneStatus == 6)
            {
                sp.Stop(4);
                await Task.Delay(500);
                if (craneStatus == 6) craneStatus = 7;
            }

            if (craneStatus == 7)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    sp.Play(5, 1);
                    craneBox.moveSpeed = 0.0015f;
                }
                if (s.CheckPos(1)) craneStatus = 8;
            }

            if (craneStatus == 8)
            {
                await Task.Delay(200);
                if (craneStatus == 8) craneStatus = 9;
            }

            if (craneStatus == 9)
            {
                if (craneBox.CheckPos(6)) craneStatus = 10;
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    if (craneStatus == 9) sp.Play(6);
                    if (isBonus) await Task.Delay(downTime[1]);
                    else await Task.Delay(downTime[0]);
                    if (craneStatus == 9) craneStatus = 10;
                }
            }

            if (craneStatus == 10)
            {
                sp.Stop(6);
                await Task.Delay(200);
                if (craneStatus == 10) craneStatus = 11;
            }

            if (craneStatus == 11)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    sp.Play(7, 1);
                    craneBox.moveSpeed = moveSpeedBackup;
                }
                if (s.CheckPos(2)) craneStatus = 12;
            }

            if (craneStatus == 12)
            {
                await Task.Delay(200);
                if (craneStatus == 12) craneStatus = 13;
            }

            if (craneStatus == 13)
            {
                sp.Play(8);
                if (!player2 && craneBox.CheckPos(5)) craneStatus = 14;
                if (player2 && craneBox.CheckPos(7)) craneStatus = 14;
            }

            if (craneStatus == 14)
            {
                sp.Stop(8);
                craneStatus = 15;
            }

            if (craneStatus == 15)
            {
                sp.Play(6);
                if (craneBox.CheckPos(6)) craneStatus = 16;
            }

            if (craneStatus == 16)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    sp.Stop(6);
                    sp.Play(9, 1);

                    isBonus = false;

                    for (int i = 0; i < 16; i++)
                        isExecuted[i] = false;

                    if (creditSystem.creditDisplayed > 0)
                        craneStatus = 1;
                    else
                        craneStatus = 0;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (craneStatus != 0)
        {
            if (craneStatus == -1)
            {
                if (!player2) craneBox.Left();
                else craneBox.Right();
                craneBox.Down();
            }
            else if (craneStatus == 2) craneBox.Up();
            else if (craneStatus == 4 || craneStatus == 5)
            {
                if (!player2) craneBox.Right();
                else craneBox.Left();
            }
            else if (craneStatus == 7)
                s.Stretch();
            else if (craneStatus == 9 || craneStatus == 15)
                craneBox.Down();
            else if (craneStatus == 11)
                s.Shrink();
            else if (craneStatus == 13)
            {
                if (!player2) craneBox.Left();
                else craneBox.Right();
            }
        }
    }

    protected override void DetectKey(int num)
    {
        if (host.playable)
        {
            int credit = 0;
            switch (num)
            {
                case 1:
                    if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonPushed && !player2)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();
                            credit = creditSystem.PlayStart();
                            // if (credit < 10) credit3d.text = credit.ToString();
                            // else credit3d.text = "9.";
                            isExecuted[16] = false;
                            probability = creditSystem.ProbabilityCheck();
                            Debug.Log("Probability:" + probability);
                        }
                        craneStatus = 2;
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) && !buttonPushed && player2)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();
                            credit = creditSystem.PlayStart();
                            // if (credit < 10) credit3d.text = credit.ToString();
                            // else credit3d.text = "9.";
                            isExecuted[16] = false;
                            probability = creditSystem.ProbabilityCheck();
                            Debug.Log("Probability:" + probability);
                        }
                        craneStatus = 2;
                    }
                    break;
                case 2:
                    if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed && !player2)
                    {
                        craneStatus = 3;
                        buttonPushed = false;
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Alpha7)) && buttonPushed && player2)
                    {
                        craneStatus = 3;
                        buttonPushed = false;
                    }
                    break;
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed && !player2)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8)) && !buttonPushed && player2)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                    }
                    break;
                case 4:
                    if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed && !player2)
                    {
                        craneStatus = 5;
                        buttonPushed = false;
                    }
                    if ((Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Alpha8)) && buttonPushed && player2)
                    {
                        craneStatus = 5;
                        buttonPushed = false;
                    }
                    break;
            }
        }
    }

    public override void ButtonDown(int num)
    {
        if (host.playable)
        {
            int credit = 0;
            switch (num)
            {
                case 1:
                    if (craneStatus == 1 && !buttonPushed)
                    {
                        buttonPushed = true;
                        craneStatus = 2;
                        creditSystem.ResetPayment();
                        credit = creditSystem.PlayStart();
                        // if (credit < 10) credit3d.text = credit.ToString();
                        // else credit3d.text = "9.";
                        isExecuted[16] = false;
                        probability = creditSystem.ProbabilityCheck();
                        Debug.Log("Probability:" + probability);
                    }
                    break;
                case 2:
                    if ((craneStatus == 3 && !buttonPushed) || (craneStatus == 4 && buttonPushed))
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                    }
                    break;
            }
        }
    }

    public void ButtonUp(int num)
    {
        if (host.playable)
        {
            switch (num)
            {
                case 1:
                    if (craneStatus == 2 && buttonPushed)
                    {
                        craneStatus = 3;
                        buttonPushed = false;
                    }
                    break;
                case 2:
                    if (craneStatus == 4 && buttonPushed)
                    {
                        craneStatus = 5;
                        buttonPushed = false;
                    }
                    break;
            }
        }
    }

    public override void InsertCoin()
    {
        if (!isHibernate && host.playable && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            // if (credit < 10) credit3d.text = credit.ToString();
            // else credit3d.text = "9.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }
}
