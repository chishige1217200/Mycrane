using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type7Manager : MonoBehaviour
{
    public int craneStatus = -1; //-1:初期化動作，0:待機状態
    public int[] priceSet = new int[2];
    public int[] timesSet = new int[2];
    float catchArmpower = 100; //掴むときのアームパワー(%，未確率時)
    float upArmpower = 100; //上昇時のアームパワー(%，未確率時)
    float backArmpower = 100; //獲得口移動時のアームパワー(%，未確率時)
    float catchArmpowersuccess = 100; //同確率時
    float upArmpowersuccess = 100; //同確率時
    float backArmpowersuccess = 100; //同確率時
    int soundType = 1; //0:CARINO 1:CARINO4 2:BAMBINO 3:neomini
    float audioPitch = 1f; //サウンドのピッチ
    private bool[] isExecuted = new bool[13]; //各craneStatusで1度しか実行しない処理の管理
    public bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    public bool probability; //確率判定用
    public int downTime = 0; //0より大きく4600以下のとき有効，下降時間設定
    [SerializeField] bool playable = true; //playableがtrueのとき操作可能
    public float armPower; //現在のアームパワー
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    BGMPlayer _BGMPlayer;
    SEPlayer _SEPlayer;
    Type3ArmController armController;
    CraneBox craneBox;
    GetPoint getPoint;
    RopeManager ropeManager;
    ArmControllerSupport support;

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    async void Start()
    {
        Transform temp;
        // 様々なコンポーネントの取得
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = this.transform.Find("CraneUnit").transform;

        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];

        // ロープとアームコントローラに関する処理
        ropeManager = this.transform.Find("RopeManager").GetComponent<RopeManager>();
        armController = temp.Find("ArmUnit").GetComponent<Type3ArmController>();
        support = temp.Find("ArmUnit").Find("Head").Find("Hat").GetComponent<ArmControllerSupport>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        //craneBox.GetManager(7);

        // ロープにマネージャー情報をセット
        ropeManager.SetManagerToPoint(7);
        creditSystem.GetSEPlayer(_SEPlayer);
        support.GetManager(7);
        support.GetRopeManager(ropeManager);
        creditSystem.SetCreditSound(0);
        _BGMPlayer.SetAudioPitch(audioPitch);
        _SEPlayer.SetAudioPitch(audioPitch);
        armController.GetManager(7);

        getPoint.GetManager(7);

        await Task.Delay(300);
        ropeManager.ArmUnitUp();
        craneBox.leftMoveFlag = true;
        craneBox.forwardMoveFlag = true;

        for (int i = 0; i < 12; i++)
            isExecuted[i] = false;

        await Task.Delay(4000);

        craneStatus = 0;
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) creditSystem.GetPayment(100);
        //craneStatusdisplayed.text = craneStatus.ToString();
        if (craneStatus == -1)
        {
            _BGMPlayer.StopBGM(soundType);
            //コイン投入無効化;
        }

        if (craneStatus == 0)
        {
            _BGMPlayer.PlayBGM(0);
            //コイン投入有効化;
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
            /*if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;*/
            //}
        }

        if (craneStatus == 1)
        {
            //コイン投入有効化;
            _BGMPlayer.StopBGM(0);
            if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
                await Task.Delay(3000);
                _SEPlayer.PlaySE(1, 1);
                await Task.Delay(3000);
                if (craneStatus == 1) craneStatus = 2;
            }

            //開始準備;
        }

        if (craneStatus == 2)
        { //移動可能
            _BGMPlayer.PlayBGM(1);
            InputKeyCheck(craneStatus);
            //コイン投入無効化;
            /*if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
            }*/
            //クレーン右移動;
            //右移動効果音ループ再生;
        }

        if (craneStatus == 3)
        {
            InputKeyCheck(craneStatus);         //奥移動ボタン有効化;
            /*if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;

            }*/
            //右移動効果音ループ再生停止;
        }

        if (craneStatus == 4)
        { //奥移動中
            InputKeyCheck(craneStatus);
            //クレーン奥移動;
            /*if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;

            }*/
            //奥移動効果音ループ再生;
        }

        if (craneStatus == 5)
        {
            if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
                await Task.Delay(1000);
                ropeManager.ArmUnitDown();
                if (craneStatus == 5) craneStatus = 6;
            }
            //奥移動効果音ループ再生停止;
            //アーム開く音再生;
            //アーム開く;
        }

        if (craneStatus == 6)
        {
            if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
                if (downTime > 0 && downTime <= 4600)
                {
                    await Task.Delay(downTime);
                    if (craneStatus == 6)
                    {
                        ropeManager.ArmUnitDownForceStop();
                        craneStatus = 7;
                    }
                }
            }
            //アーム下降音再生
            //アーム下降;
        }

        if (craneStatus == 7)
        {
            if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
                if (probability) armPower = catchArmpowersuccess;
                else armPower = catchArmpower;
                armController.MotorPower(armPower);
                armController.ArmClose();
                await Task.Delay(1000);
                if (craneStatus == 7) craneStatus = 8;
            }
            //アーム下降音再生停止;
            //アーム掴む音再生;
            //アーム掴む;
        }

        if (craneStatus == 8)
        {
            if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
                ropeManager.ArmUnitUp();
                await Task.Delay(1500);
                if (!probability && UnityEngine.Random.Range(0, 2) == 0 && craneStatus == 8 && support.prizeFlag) armController.Release(); // 上昇中に離す振り分け
            }
            if (probability && armPower > upArmpowersuccess)
            {
                armPower -= 0.5f;
                armController.MotorPower(armPower);
            }
            else if (!probability && armPower > upArmpower)
            {
                armPower -= 0.5f;
                armController.MotorPower(armPower);
            }
            //アーム上昇音再生;
            //アーム上昇;
        }

        if (craneStatus == 9)
        {
            if (probability) armPower = upArmpowersuccess;
            else armPower = upArmpower;
            armController.MotorPower(armPower);
            if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
                if (craneStatus == 9) craneStatus = 10;
            }
            //アーム上昇停止音再生;
            //アーム上昇停止;
        }

        if (craneStatus == 10)
        {
            //アーム獲得口ポジション移動音再生;
            if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
                craneBox.leftMoveFlag = true;
                craneBox.forwardMoveFlag = true;
            }
            if (probability && armPower > backArmpowersuccess)
            {
                armPower -= 0.5f;
                armController.MotorPower(armPower);
            }
            else if (!probability && armPower > backArmpower)
            {
                armPower -= 0.5f;
                armController.MotorPower(armPower);
            }
            if (craneBox.CheckPos(1) && craneStatus == 10) craneStatus = 11;
            //アーム獲得口ポジションへ;
        }

        if (craneStatus == 11)
        {
            if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
                armController.ArmOpen();
                await Task.Delay(2000);
                if (craneStatus == 11) craneStatus = 12;
            }
            //アーム開く音再生;
            //アーム開く;
            //1秒待機;
        }

        if (craneStatus == 12)
        {

            if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
                armController.ArmFinalClose();

                for (int i = 0; i < 12; i++)
                    isExecuted[i] = false;
                await Task.Delay(1000);
                if (creditSystem.creditDisplayed > 0)
                    craneStatus = 1;
                else
                    craneStatus = 0;
                //アーム閉じる音再生;
                //アーム閉じる;
            }
        }
    }

    public void GetPrize()
    {
        switch (creditSystem.probabilityMode)
        {
            case 2:
            case 3:
                creditSystem.ResetCreditProbability();
                break;
            case 4:
            case 5:
                creditSystem.ResetCostProbability();
                break;
        }
        /*
                if (!_SEPlayer._AudioSource[getSoundNum].isPlaying)
                {
                    if (getSoundNum != -1)
                        _SEPlayer.PlaySE(getSoundNum, 1);
                }*/ //getSoundNumに定数を入れる
    }

    public void InputKeyCheck(int num)
    {
        switch (num)
        {
            case 1:
                if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) && !buttonPushed)
                {
                    buttonPushed = true;
                    if (craneStatus == 1)
                    {
                        creditSystem.ResetNowPayment();
                        creditSystem.AddCreditPlayed();
                        isExecuted[12] = false;
                        probability = creditSystem.ProbabilityCheck();
                        Debug.Log("Probability:" + probability);
                    }
                    craneStatus = 2;
                    craneBox.rightMoveFlag = true;
                }
                break;
            //投入を無効化
            case 2:
                if ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Alpha1)) && buttonPushed)
                {
                    craneStatus = 3;
                    craneBox.rightMoveFlag = false;
                    buttonPushed = false;
                }
                break;
            case 3:
                if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) && !buttonPushed)
                {
                    buttonPushed = true;
                    craneStatus = 4;
                    craneBox.backMoveFlag = true;
                }
                break;
            case 4:
                if ((Input.GetKeyUp(KeyCode.Keypad2) || Input.GetKeyUp(KeyCode.Alpha2)) && buttonPushed)
                {
                    craneStatus = 5;
                    craneBox.backMoveFlag = false;
                    buttonPushed = false;
                }
                break;
        }
    }

    public void ButtonDown(int num)
    {
        switch (num)
        {
            case 1:
                if (craneStatus == 1 && !buttonPushed)
                {
                    buttonPushed = true;
                    craneStatus = 2;
                    creditSystem.ResetNowPayment();
                    creditSystem.AddCreditPlayed();
                    isExecuted[12] = false;
                    probability = creditSystem.ProbabilityCheck();
                    Debug.Log("Probability:" + probability);
                }
                if (craneStatus == 2 && buttonPushed)
                    craneBox.rightMoveFlag = true;
                break;
            case 2:
                if ((craneStatus == 3 && !buttonPushed) || (craneStatus == 4 && buttonPushed))
                {
                    buttonPushed = true;
                    craneStatus = 4;
                    craneBox.backMoveFlag = true;
                }
                break;
        }
    }

    public void ButtonUp(int num)
    {
        switch (num)
        {
            case 1:
                if (/*craneStatus == 1 ||*/ (craneStatus == 2 && buttonPushed))
                {
                    craneStatus = 3;
                    craneBox.rightMoveFlag = false;
                    buttonPushed = false;
                }
                break;
            case 2:
                if (/*craneStatus == 3 ||*/ (craneStatus == 4 && buttonPushed))
                {
                    craneStatus = 5;
                    craneBox.backMoveFlag = false;
                    buttonPushed = false;
                }
                break;
        }
    }

    public void Testadder()
    {
        Debug.Log("Clicked.");
        craneStatus++;
    }

    public void TestSubber()
    {
        Debug.Log("Clicked.");
        craneStatus--;
    }
}
