using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrasJewelManager : CraneManagerV2
{
    public bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] bool player2 = false; //player2の場合true
    LockOnProbabilityChecker lc;
    LockOnStretcher ls;
    GameObject internalCamera;
    GameObject blackLine;
    [SerializeField] TextMesh credit3d;
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
        craneType = 98;

        // 様々なコンポーネントの取得
        canvas = transform.Find("Canvas").gameObject;
        // creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystem>();
        getPoint = transform.Find("GetPoint").GetComponent<GetPointV2>();
        temp = transform.Find("CraneUnit").Find("CraneBox").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];
        if (isHibernate)
        {
            credit3d.text = "--";
            creditSystem.SetHibernate();
        }

        // CraneBoxに関する処理
        // craneBox = temp.GetComponent<CraneBox>();
        lc = temp.Find("JudgePoint").GetComponent<LockOnProbabilityChecker>();
        if (!player2)
        {
            ls = temp.Find("TETRAS").Find("Stretcher").GetComponent<LockOnStretcher>();
        }
        else
        {
            ls = temp.Find("JEWEL").Find("Stretcher").GetComponent<LockOnStretcher>();
        }
        internalCamera = temp.Find("Camera").gameObject;
        blackLine = canvas.transform.Find("ControlGroup").Find("Black Line").gameObject;

        creditSystem.SetSEPlayer(sp);
        creditSystem.SetCreditSound(0);
        getPoint.SetManager(this);

        craneStatus = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (useUI && host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf)
        {
            canvas.SetActive(false);
            if (internalCamera.activeSelf)
            {
                MultiCamera(false);
            }
        }
        if (host.playable && !player2 && Input.GetKeyDown(KeyCode.LeftBracket))
        {
            MultiCamera(!internalCamera.activeSelf);
        }
        if (host.playable && player2 && Input.GetKeyDown(KeyCode.RightBracket))
        {
            MultiCamera(!internalCamera.activeSelf);
        }

        if (!player2 && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();
        else if (player2 && (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus))) InsertCoin();

        if (craneStatus == -1)
        {
            if (craneBox.CheckPos(1) && !player2) craneStatus = 0;
            if (craneBox.CheckPos(3) && player2) craneStatus = 0;
        }

        if (craneStatus > 0)
        {
            if (craneStatus == 1)
            {
                // 待機中
                DetectKey(craneStatus);
            }

            if (craneStatus == 2)
            {
                // 横移動中
                DetectKey(craneStatus);
                if (!player2 && craneBox.CheckPos(7))
                {
                    buttonPushed = false;
                    craneStatus = 3;
                }
                if (player2 && craneBox.CheckPos(5))
                {
                    buttonPushed = false;
                    craneStatus = 3;
                }
            }

            if (craneStatus == 3)
            {
                // 待機中
                DetectKey(craneStatus);
            }

            if (craneStatus == 4)
            {
                // 上移動中
                DetectKey(craneStatus);
                if (craneBox.CheckPos(8))
                {
                    buttonPushed = false;
                    craneStatus = 5;
                }
            }

            if (craneStatus == 5)
            {
                // 滑り動作
                if (!lc.GetInJudge()) craneStatus = 6;
            }

            if (craneStatus == 7)
            {
                // 伸びる動作
                if (ls.CheckPos(1)) craneStatus = 8;
            }

            if (craneStatus == 9)
            {
                // 縮む動作
                if (ls.CheckPos(2))
                {
                    if (craneStatus == 9) craneStatus = 10;
                }
            }

            if (craneStatus == 11)
            {
                if (craneBox.CheckPos(1) && !player2)
                {
                    craneStatus = 12;
                }
                if (craneBox.CheckPos(3) && player2)
                {
                    craneStatus = 12;
                }
            }

            if (craneStatus == 14)
            {
                if (craneBox.CheckPos(1))
                {
                    if (creditSystem.creditDisplayed > 0)
                        craneStatus = 1;
                    else
                        craneStatus = 0;
                }
                if (craneBox.CheckPos(3) && player2)
                {
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
            if (craneStatus == -1 || craneStatus == 10 || craneStatus == 11)
            {
                if (!player2) craneBox.Left();
                else craneBox.Right();
            }
            if (craneStatus == 2)
            {
                if (!player2) craneBox.Right();
                else craneBox.Left();
            }
            if (craneStatus == 4 || craneStatus == 5 || craneStatus == 10 || craneStatus == 13) craneBox.Up();
            if (craneStatus == -1 || craneStatus == 11 || craneStatus == 14) craneBox.Down();

            if (craneStatus == 7) ls.Stretch();
            if (craneStatus == 9) ls.Shrink();
        }
    }

    public override void GetPrize()
    {
        sp.Play(1, 1);
    }

    protected override void DetectKey(int num)
    {
        if (host.playable)
        {
            switch (num)
            {
                case 1:
                    if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonPushed && !player2)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();
                            int credit = creditSystem.PlayStart();
                            if (credit < 100) credit3d.text = credit.ToString("D2");
                            else credit3d.text = "99.";
                            lc.ResetJudge();
                            //probability = creditSystem.ProbabilityCheck();
                            //Debug.Log("Probability:" + probability);
                        }
                        craneStatus = 2;
                    }
                    if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) && !buttonPushed && player2)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();
                            int credit = creditSystem.PlayStart();
                            if (credit < 100) credit3d.text = credit.ToString("D2");
                            else credit3d.text = "99.";
                            lc.ResetJudge();
                            //probability = creditSystem.ProbabilityCheck();
                            //Debug.Log("Probability:" + probability);
                        }
                        craneStatus = 2;
                    }
                    break;
                //投入を無効化
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
            switch (num)
            {
                case 1:
                    if (craneStatus == 1 && !buttonPushed)
                    {
                        buttonPushed = true;
                        craneStatus = 2;
                        creditSystem.ResetPayment();
                        int credit = creditSystem.PlayStart();
                        if (credit < 100) credit3d.text = credit.ToString("D2");
                        else credit3d.text = "99.";
                        lc.ResetJudge();
                        //probability = creditSystem.ProbabilityCheck();
                        //Debug.Log("Probability:" + probability);
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
            if (credit < 100) credit3d.text = credit.ToString("D2");
            else credit3d.text = "99.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }

    protected override void FirstStatusEvent(int status)
    {
        switch (status)
        {
            case 1:
                craneBox.moveSpeed = 0.0025f;
                break;
            case 6:
                StartCoroutine(DelayCoroutine(500, () =>
                {
                    craneStatus = 7;
                }));
                break;
            case 8:
                lc.IncrimentTarget();
                StartCoroutine(DelayCoroutine(500, () =>
                {
                    craneStatus = 9;
                }));
                break;
            case 10:
                StartCoroutine(DelayCoroutine(500, () =>
                {
                    craneStatus = 11;
                }));
                break;
            case 12:
                StartCoroutine(DelayCoroutine(500, () =>
                {
                    craneStatus = 13;
                }));
                break;
            case 13:
                craneBox.moveSpeed = 0.002f;
                StartCoroutine(DelayCoroutine(500, () =>
                {
                    craneStatus = 14;
                }));
                break;
        }
    }

    protected override void LastStatusEvent(int status)
    {
    }

    public void MultiCamera(bool flag)
    {
        internalCamera.SetActive(flag);
        blackLine.SetActive(flag);
    }
}
