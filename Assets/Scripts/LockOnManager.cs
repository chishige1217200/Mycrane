using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class LockOnManager : CraneManager
{
    [SerializeField] int[] priceSet = new int[2];
    [SerializeField] int[] timesSet = new int[2];
    bool[] isExecuted = new bool[11]; //各craneStatusで1度しか実行しない処理の管理
    bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    BGMPlayer bp;
    LockOnProbabilityChecker lc;
    LockOnStretcher ls;
    GameObject[] internalCamera = new GameObject[2];
    GameObject blackLine;
    [SerializeField] TextMesh credit3d;
    // Start is called before the first frame update
    async void Start()
    {
        Transform temp;

        craneStatus = -2;
        craneType = 99;

        // 様々なコンポーネントの取得
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystem>();
        bp = transform.Find("BGM").GetComponent<BGMPlayer>();
        temp = transform.Find("CraneUnit").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];
        if (isHibernate)
        {
            credit3d.text = "-";
            creditSystem.SetHibernate();
        }

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        lc = temp.Find("CraneBox").Find("JudgePoint").GetComponent<LockOnProbabilityChecker>();
        ls = temp.Find("CraneBox").Find("Stretcher").GetComponent<LockOnStretcher>();
        internalCamera[0] = temp.Find("CraneBox").Find("Camera").gameObject;
        internalCamera[1] = temp.Find("CraneBox").Find("Camera (1)").gameObject;
        blackLine = canvas.transform.Find("ControlGroup").Find("Black Line").gameObject;

        // ロープにマネージャー情報をセット
        creditSystem.SetSEPlayer(sp);
        creditSystem.SetCreditSound(0);

        await Task.Delay(300);

        for (int i = 0; i < 11; i++)
            isExecuted[i] = false;

        craneStatus = -1;
        lc.ResetJudge();
    }

    // Update is called once per frame
    async void Update()
    {
        if (host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf)
        {
            canvas.SetActive(false);
            if (internalCamera[0].activeSelf)
            {
                MultiCamera(false);
            }
        }
        if (host.playable && (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadPeriod)))
        {
            MultiCamera(!internalCamera[0].activeSelf);
        }
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();

        if (craneStatus == -1)
            if (craneBox.CheckPos(1)) craneStatus = 0;

        if (craneStatus == 0)
        {
            //コイン投入有効化;
            if (!sp.audioSource[2].isPlaying && !sp.audioSource[3].isPlaying) bp.Play(0);
        }
        else
        {
            if (craneStatus == 1)
            {
                // 待機中
                DetectKey(craneStatus);
                if (!sp.audioSource[0].isPlaying && !sp.audioSource[2].isPlaying && !sp.audioSource[3].isPlaying) bp.Play(0);
            }

            if (craneStatus == 2)
            {
                // 上移動中
                bp.Stop(0);
                bp.Play(1);
                DetectKey(craneStatus);
                if (craneBox.CheckPos(8))
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
                // 右移動中
                bp.Stop(1);
                bp.Play(2);
                DetectKey(craneStatus);
                if (craneBox.CheckPos(7))
                {
                    buttonPushed = false;
                    craneStatus = 5;
                }
            }

            if (craneStatus == 5)
            {
                // 滑り動作
                if (!lc.GetInJudge()) craneStatus = 6;

                /*if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    await Task.Delay(1000);
                    if (craneStatus == 5) craneStatus = 6;
                }
                if (craneBox.CheckPos(7))
                {
                    buttonPushed = false;
                    if (craneStatus == 5) craneStatus = 6;
                }*/
            }

            if (craneStatus == 6)
            {
                // 待機動作
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    bp.Stop(2);
                    sp.Play(1, 1);
                    await Task.Delay(1000);
                    if (craneStatus == 6) craneStatus = 7;
                }
            }

            if (craneStatus == 7)
            {
                // 伸びる動作
                if (ls.CheckPos(1)) craneStatus = 8;
            }

            if (craneStatus == 8)
            {
                // 待機動作
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    lc.IncrimentTarget();
                    sp.Stop(1);
                    if (!sp.audioSource[3].isPlaying) sp.Play(2, 1);
                    await Task.Delay(1000);
                    if (craneStatus == 8) craneStatus = 9;
                }
            }

            if (craneStatus == 9)
            {
                // 縮む動作
                if (ls.CheckPos(2))
                {
                    if (craneStatus == 9) craneStatus = 10;
                }
            }

            if (craneStatus == 10)
            {
                if (craneBox.CheckPos(1))
                {
                    if (!isExecuted[craneStatus])
                    {
                        isExecuted[craneStatus] = true;
                        for (int i = 0; i < 10; i++)
                            isExecuted[i] = false;
                        if (creditSystem.creditDisplayed > 0)
                            craneStatus = 1;
                        else
                            craneStatus = 0;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (craneStatus != 0)
        {
            if (craneStatus == -1 || craneStatus == 10)
            {
                craneBox.Left();
                craneBox.Down();
            }
            else if (craneStatus == 2) craneBox.Up();
            else if (craneStatus == 4 || craneStatus == 5) craneBox.Right();
            if (craneStatus == 7) ls.Stretch();
            if (craneStatus == 9) ls.Shrink();
        }
    }

    public override async void GetPrize()
    {
        await Task.Delay(500);
        sp.Stop(1);
        if (!sp.audioSource[3].isPlaying) sp.Play(3, 1);
    }

    protected override void DetectKey(int num)
    {
        if (host.playable)
        {
            switch (num)
            {
                case 1:
                    if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonPushed)
                    {
                        buttonPushed = true;
                        if (craneStatus == 1)
                        {
                            creditSystem.ResetPayment();
                            int credit = creditSystem.PlayStart();
                            if (credit < 10) credit3d.text = credit.ToString();
                            else credit3d.text = "9.";
                            lc.ResetJudge();
                            isExecuted[10] = false;
                            //probability = creditSystem.ProbabilityCheck();
                            //Debug.Log("Probability:" + probability);
                        }
                        craneStatus = 2;
                    }
                    break;
                //投入を無効化
                case 2:
                    if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed)
                    {
                        craneStatus = 3;
                        buttonPushed = false;
                    }
                    break;
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed)
                    {
                        buttonPushed = true;
                        craneStatus = 4;
                    }
                    break;
                case 4:
                    if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed)
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
                        if (credit < 10) credit3d.text = credit.ToString();
                        else credit3d.text = "9.";
                        lc.ResetJudge();
                        isExecuted[10] = false;
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
            bp.Stop(0);
            int credit = creditSystem.Pay(100);
            if (credit < 10) credit3d.text = credit.ToString();
            else credit3d.text = "9.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
        }
    }

    public void MultiCamera(bool flag)
    {
        if (flag)
        {
            internalCamera[0].SetActive(true);
            internalCamera[1].SetActive(true);
            blackLine.SetActive(true);
        }
        else
        {
            internalCamera[0].SetActive(false);
            internalCamera[1].SetActive(false);
            blackLine.SetActive(false);
        }
    }

    public void MultiCameraAuto()
    {
        internalCamera[0].SetActive(!internalCamera[0].activeSelf);
        internalCamera[1].SetActive(internalCamera[0].activeSelf);
        blackLine.SetActive(internalCamera[0].activeSelf);
    }
}
