using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Type3ManagerV3 : CraneManagerV3
{
    [SerializeField] float[] armPowerConfig = new float[3]; // アームパワー(%，未確率時)
    [SerializeField] float[] armPowerConfigSuccess = new float[3]; // アームパワー(%，確率時)
    [SerializeField] int soundType = 1; // 0:CARINO 1:CARINO4 2:BAMBINO 3:neomini
    [SerializeField] float audioPitch = 1f; // サウンドのピッチ
    [SerializeField] int downTime = 0; // 0より大きく4600以下のとき有効，下降時間設定
    [SerializeField] bool autoPower = true;
    [SerializeField] bool openEnd = false;
    [HideInInspector] public float armPower; // 現在のアームパワー
    private bool buttonPushed = false; // trueならボタンをクリックしているかキーボードを押下している
    private BGMPlayer bp;
    private Type3ArmControllerV3 armController;
    private BaseLifter ropeManager;
    private ArmControllerSupportV3 support;
    [SerializeField] TextMesh credit3d;
    [SerializeField] GameObject wait3d;
    private IEnumerator DelayCoroutine(float miliseconds, Action action)
    {
        yield return new WaitForSeconds(miliseconds / 1000f);
        action?.Invoke();
    }

    void Start()
    {
        Transform temp;

        craneStatus = -1;
        craneType = 3;

        // 様々なコンポーネントの取得
        canvas = transform.Find("Canvas").gameObject;
        creditSystem = transform.Find("CreditSystem").GetComponent<CreditSystemV3>();
        bp = transform.Find("BGM").GetComponent<BGMPlayer>();
        sp = transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPointV3>();
        temp = transform.Find("CraneUnit").transform;

        // クレジット情報登録

        if (isHibernate)
        {
            credit3d.text = "-";
            creditSystem.SetHibernate();
        }

        // ロープとアームコントローラに関する処理
        ropeManager = transform.Find("RopeManager").GetComponent<BaseLifter>();
        armController = temp.Find("ArmUnit").GetComponent<Type3ArmControllerV3>();
        support = temp.Find("ArmUnit").Find("Head").Find("Hat").GetComponent<ArmControllerSupportV3>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBoxV3>();

        // ロープにマネージャー情報をセット
        creditSystem.Setup();
        creditSystem.SetSEPlayer(sp);
        support.SetManager(this);
        support.SetLifter(ropeManager);
        if (soundType == 0) creditSystem.SetSoundNum(0);
        if (soundType == 1) creditSystem.SetSoundNum(6);
        if (soundType == 2) creditSystem.SetSoundNum(13);
        if (soundType == 3) creditSystem.SetSoundNum(-1);
        bp.SetAudioPitch(audioPitch);
        sp.SetAudioPitch(audioPitch);
        armController.SetManager(3);
        armController.autoPower = autoPower;

        getPoint.SetManager(this);

        host.manualCode = 4;

        StartCoroutine(Init());
    }

    // 基本動作チェック用
    IEnumerator Init()
    {
        Debug.Log("Starting...");
        yield return new WaitForSeconds(3);
        // ropeManager.Up();

        // while (true)
        // {
        //     if (ropeManager.UpFinished()) break;
        //     yield return null;
        // }

        // yield return new WaitForSeconds(1);

        // craneBox.Right(true);
        // yield return new WaitForSeconds(1);
        // craneBox.Right(false);
        // yield return new WaitForSeconds(0.5f);
        // craneBox.Left(true);
        // yield return new WaitForSeconds(1);
        // craneBox.Left(false);
        // yield return new WaitForSeconds(0.5f);

        // craneBox.Right(true);

        // while (true)
        // {
        //     if (craneBox.CheckPos(7)) break;
        //     yield return null;
        // }

        // yield return new WaitForSeconds(0.5f);

        // craneBox.Back(true);

        // while (true)
        // {
        //     if (craneBox.CheckPos(4)) break;
        //     yield return null;
        // }

        // yield return new WaitForSeconds(0.5f);

        // armController.Open();

        // yield return new WaitForSeconds(1);

        // ropeManager.Down();

        // while (true)
        // {
        //     if (ropeManager.DownFinished()) break;
        //     yield return null;
        // }

        // yield return new WaitForSeconds(1);

        // ropeManager.Up();

        // while (true)
        // {
        //     if (ropeManager.UpFinished()) break;
        //     yield return null;
        // }

        // yield return new WaitForSeconds(1);

        // armController.Close();

        // yield return new WaitForSeconds(0.5f);

        // craneBox.Left(true);

        // while (true)
        // {
        //     if (craneBox.CheckPos(2)) break;
        //     yield return null;
        // }

        // yield return new WaitForSeconds(0.5f);

        // craneBox.Forward(true);

        // while (true)
        // {
        //     if (craneBox.CheckPos(1)) break;
        //     yield return null;
        // }

        // armController.Open();

        // yield return new WaitForSeconds(3);

        // armController.Close();

        // yield return new WaitForSeconds(3);

        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {
        // ロープ巻取り確認
        if (!ropeManager.UpFinished())
        {
            ropeManager.Up();
            while (!ropeManager.UpFinished())
            {
                yield return null;
            }
        }

        // アームユニット位置確認
        if (!craneBox.CheckPos(1))
        {
            craneBox.Left(true);
            craneBox.Forward(true);
            while (!craneBox.CheckPos(1))
            {
                yield return null;
            }
        }

        if (openEnd) armController.Open();
        else armController.Close();

        craneStatus = 0;
    }

    void Update()
    {
        if (useUI && host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();

        if (craneStatus == 0)
        {
            switch (soundType)
            {
                case 0:
                    if (!sp.audioSource[5].isPlaying) bp.Play(0);
                    break;
                case 1:
                    if (!sp.audioSource[6].isPlaying && !sp.audioSource[12].isPlaying) bp.Play(1);
                    break;
                case 2:
                    if (!sp.audioSource[13].isPlaying && !sp.audioSource[16].isPlaying && !sp.audioSource[17].isPlaying)
                        bp.Play(2);
                    break;
                case 3:
                    bp.Stop(4);
                    bp.Play(3);
                    break;
            }
        }
        else if (craneStatus == 1)
        {
            switch (soundType)
            {
                case 1:
                    if (!sp.audioSource[6].isPlaying)
                        sp.Play(7);
                    break;
                case 2:
                    if (!sp.audioSource[13].isPlaying && !sp.audioSource[16].isPlaying && !sp.audioSource[17].isPlaying)
                        bp.Play(2);
                    break;
                case 3:
                    bp.Play(4);
                    break;
            }
        }

        if (craneStatus >= 1 && craneStatus <= 4) DetectKey(craneStatus);

        if (craneBox.CheckPos(7) && craneStatus == 2)
        {
            buttonPushed = false;
            craneStatus = 3;
        }
        if (craneBox.CheckPos(8) && craneStatus == 4)
        {
            buttonPushed = false;
            craneStatus = 5;
        }
        if (ropeManager.DownFinished() && craneStatus == 6) craneStatus = 7;
        if (soundType == 2 && craneStatus == 8)
            if (!sp.audioSource[15].isPlaying)
                sp.Play(14);
        if (ropeManager.UpFinished() && craneStatus == 8) craneStatus = 9;
        if (craneBox.CheckPos(1) && craneStatus == 10) craneStatus = 11;
    }

    void FixedUpdate() // アームのパワーを緩める処理のみ残す
    {
        if (craneStatus != 0)
        {
            if (craneStatus == 8)
            {
                if (!armController.autoPower)
                {
                    if (support.prizeCount > 0)
                    {
                        if (probability && armPower > armPowerConfigSuccess[1]) armPower -= 0.5f;
                        else if (!probability && armPower > armPowerConfig[1]) armPower -= 0.5f;
                        armController.MotorPower(armPower);
                    }
                    else armController.MotorPower(100f);
                }
            }
            else if (craneStatus == 10)
            {
                if (!armController.autoPower)
                {
                    if (support.prizeCount > 0)
                    {
                        if (probability && armPower > armPowerConfigSuccess[2]) armPower -= 0.5f;
                        else if (!probability && armPower > armPowerConfig[2]) armPower -= 0.5f;
                        armController.MotorPower(armPower);
                    }
                    else armController.MotorPower(100f);
                }
            }

        }
    }

    public override void GetPrize()
    {
        switch (soundType)
        {
            case 0:
                getSoundNum = 5;
                sp.Stop(1);
                break;
            case 1:
                getSoundNum = 12;
                sp.Stop(11);
                break;
            case 2:
                getSoundNum = 16;
                sp.Stop(14);
                sp.Stop(17);
                break;
            case 3:
                getSoundNum = 26;
                sp.Stop(25);
                break;
        }

        base.GetPrize();
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
                        ButtonDown(1);
                    }
                    break;
                case 2:
                    if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed)
                    {
                        ButtonUp(1);
                    }
                    break;
                case 3:
                    if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed)
                    {
                        ButtonDown(2);
                    }
                    break;
                case 4:
                    if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed)
                    {
                        ButtonUp(2);
                    }
                    break;
            }
        }
    }

    public override void ButtonDown(int num)
    {
        switch (num)
        {
            case 1:
                if (craneStatus == 1 && !buttonPushed)
                {
                    buttonPushed = true;
                    craneStatus = 2;
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

    public override void ButtonUp(int num)
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

    public override void InsertCoin()
    {
        if (!isHibernate && host.playable && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 10) credit3d.text = credit.ToString();
            else credit3d.text = "9.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
            if (soundType == 1) bp.Stop(1);
            if (soundType == 2) bp.Stop(2);
        }
    }

    public override void InsertCoinAuto()
    {
        if (!isHibernate && craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);
            if (credit < 10) credit3d.text = credit.ToString();
            else credit3d.text = "9.";
            if (credit > 0 && craneStatus == 0) craneStatus = 1;
            if (soundType == 1) bp.Stop(1);
            if (soundType == 2) bp.Stop(2);
        }
    }

    protected override void FirstStatusEvent(int status)
    {
        if (status == 0)
        {
            credit3d.gameObject.SetActive(false);
            wait3d.SetActive(true);
        }
        if (status == 1)
        {
            wait3d.SetActive(false);
            credit3d.gameObject.SetActive(true);
            bp.Stop(soundType);
        }
        else if (status == 2)
        {
            creditSystem.ResetPay();
            creditSystem.NewPlay();
            probability = creditSystem.ProbabilityCheck();
            Debug.Log("Probability:" + probability);
            craneBox.Right(true);
            switch (soundType)
            {
                case 0:
                    sp.Play(1);
                    break;
                case 1:
                    sp.Stop(7);
                    sp.Play(8);
                    break;
                case 2:
                    bp.Stop(2);
                    sp.Play(14);
                    break;
                case 3:
                    sp.Play(18);
                    break;
            }
        }
        else if (status == 3)
        {
            craneBox.Right(false);
        }
        else if (status == 4)
        {
            craneBox.Back(true);
            switch (soundType)
            {
                case 0:
                    sp.Stop(1);
                    sp.Play(2);
                    break;
                case 1:
                    sp.Stop(8);
                    sp.Play(9);
                    break;
                case 2:
                    sp.Play(14);
                    break;
                case 3:
                    sp.Stop(18);
                    sp.Play(19);
                    break;
            }
        }
        else if (status == 5)
        {
            craneBox.Back(false);
            int waitTime = 0;
            if (!openEnd) armController.Open();
            switch (soundType)
            {
                case 0:
                    sp.Stop(2);
                    sp.Play(3);
                    waitTime = 1000;
                    break;
                case 1:
                    sp.Stop(9);
                    sp.Play(10);
                    waitTime = 1000;
                    break;
                case 2:
                    sp.Stop(14);
                    sp.Play(15, 1);
                    if (!openEnd) waitTime = 1000;
                    break;
                case 3:
                    sp.Stop(19);
                    sp.Play(20, 1);
                    waitTime = 2000;
                    break;
            }
            StartCoroutine(DelayCoroutine(waitTime, () =>
            {
                ropeManager.Down();
                if (craneStatus == 5) craneStatus = 6;
            }));
        }
        else if (status == 6)
        {
            if (soundType == 3) sp.Play(21);
            if (downTime > 0 && downTime <= 4600)
            {
                StartCoroutine(DelayCoroutine(downTime, () =>
                {
                    if (craneStatus == 6)
                    {
                        ropeManager.DownForceStop();
                        craneStatus = 7;
                    }
                }));
            }
        }
        else if (status == 7)
        {
            switch (soundType)
            {
                case 3:
                    sp.Stop(21);
                    break;
            }
            if (probability) armPower = armPowerConfigSuccess[0];
            else armPower = armPowerConfig[0];
            armController.Close();
            armController.MotorPower(armPower);
            StartCoroutine(DelayCoroutine(1000, () =>
            {
                if (craneStatus == 7) craneStatus = 8;
            }));
        }
        else if (status == 8)
        {
            switch (soundType)
            {
                case 0:
                    sp.Stop(3);
                    sp.Play(4);
                    break;
                case 1:
                    sp.Stop(10);
                    sp.Play(11);
                    break;
                case 3:
                    sp.Play(22);
                    break;
            }
            ropeManager.Up();
            StartCoroutine(DelayCoroutine(1500, () =>
            {
                if (!probability && UnityEngine.Random.Range(0, 3) == 0 && craneStatus == 8 && support.prizeCount > 0) armController.Release(); //上昇中に離す振り分け(autoPower設定時のみ)
            }));
        }
        else if (status == 9)
        {
            if (!armController.autoPower)
            {
                if (probability) armPower = armPowerConfigSuccess[1];
                else armPower = armPowerConfig[1];
                armController.MotorPower(armPower);
            }
            StartCoroutine(DelayCoroutine(200, () =>
            {
                if (soundType == 3) sp.Stop(22);
                if (!probability && UnityEngine.Random.Range(0, 2) == 0 && craneStatus == 9 && support.prizeCount > 0) armController.Release(); // 上昇後に離す振り分け
                if (craneStatus == 9) craneStatus = 10;
            }));
        }
        else if (status == 10)
        {
            craneBox.Left(true);
            craneBox.Forward(true);
            switch (soundType)
            {
                case 0:
                    sp.Stop(4);
                    sp.Play(1);
                    break;
                case 2:
                    sp.Stop(15);
                    sp.Stop(14);
                    sp.Play(14);
                    break;
                case 3:
                    sp.Play(23);
                    break;
            }
        }
        else if (status == 11)
        {
            StartCoroutine(DelayCoroutine(500, () =>
            {
                armController.Open();
                int waitTime = 1500;
                switch (soundType)
                {
                    case 3:
                        sp.Stop(23);
                        sp.Play(24, 1);
                        waitTime = 2500;
                        break;
                }
                StartCoroutine(DelayCoroutine(waitTime, () =>
                {
                    craneStatus = 12;
                }));
            }));
        }
        else if (status == 12)
        {
            if (!openEnd) armController.Close();
            int waitTime = 0;
            switch (soundType)
            {
                case 0:
                case 1:
                    waitTime = 2000;
                    break;
                case 2:
                    if (openEnd && !sp.audioSource[16].isPlaying)
                    {
                        waitTime = 2000;
                        sp.Stop(14);
                        sp.Play(17, 1);
                    }
                    else if (!openEnd)
                    {
                        waitTime = 3000;
                        StartCoroutine(DelayCoroutine(1000, () =>
                        {
                            if (!sp.audioSource[16].isPlaying)
                            {
                                sp.Stop(14);
                                sp.Play(17, 1);
                            }
                        }));
                    }
                    break;
                case 3:
                    waitTime = 2500;
                    sp.Play(25, 1);
                    break;
            }
            StartCoroutine(DelayCoroutine(waitTime, () =>
            {
                switch (soundType)
                {
                    case 0:
                        sp.Stop(1);
                        break;
                    case 1:
                        sp.Stop(11);
                        break;
                }

                int credit = creditSystem.DecrementCredit();
                if (credit < 10) credit3d.text = credit.ToString();
                else credit3d.text = "9.";

                if (creditSystem.GetCreditCount() > 0)
                    craneStatus = 1;
                else
                    craneStatus = 0;
            }));
        }
    }

    protected override void LastStatusEvent(int status)
    {
    }
}
