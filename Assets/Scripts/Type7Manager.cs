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
    [SerializeField] float catchArmpower = 100; //掴むときのアームパワー(%，未確率時)
    [SerializeField] float upArmpower = 100; //上昇時のアームパワー(%，未確率時)
    [SerializeField] float backArmpower = 100; //獲得口移動時のアームパワー(%，未確率時)
    [SerializeField] float catchArmpowersuccess = 100; //同確率時
    [SerializeField] float upArmpowersuccess = 100; //同確率時
    [SerializeField] float backArmpowersuccess = 100; //同確率時
    [SerializeField] int limitTimeSet = 60; //操作制限時間
    bool[] isExecuted = new bool[13]; //各craneStatusで1度しか実行しない処理の管理
    public bool probability; //確率判定用
    float armPower; //現在のアームパワー
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    BGMPlayer _BGMPlayer;
    SEPlayer _SEPlayer;
    Type3ArmController armController;
    CraneBox craneBox;
    GetPoint getPoint;
    RopeManager ropeManager;
    ArmControllerSupport support;
    Lever[] lever = new Lever[2];
    Timer timer;
    MachineHost host;
    GameObject canvas;
    int leverState = 0; // 0:ニュートラル，1:下降中，2:上昇中
    int armState = 0; // 0:閉じている，1:開いている

    //For test-----------------------------------------

    public Text craneStatusdisplayed;
    public TextMesh limitTimedisplayed;

    //-------------------------------------------------

    async void Start()
    {
        Transform temp;
        // 様々なコンポーネントの取得
        host = this.transform.Find("CP").GetComponent<MachineHost>();
        canvas = this.transform.Find("Canvas").gameObject;
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        lever[0] = this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 1").GetComponent<Lever>();
        lever[1] = this.transform.Find("Canvas").Find("ControlGroup").Find("Lever 2").GetComponent<Lever>();
        getPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = this.transform.Find("CraneUnit").transform;
        timer = this.transform.Find("Timer").GetComponent<Timer>();

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
        timer.limitTimeNow = limitTimeSet;
        timer.limitTime = limitTimeSet;
        support.GetManager(7);
        support.GetRopeManager(ropeManager);
        creditSystem.SetCreditSound(0);
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
        limitTimedisplayed.text = timer.limitTimeNow.ToString("D2");
        if (host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0)) InsertCoin();
        //craneStatusdisplayed.text = craneStatus.ToString();
        if (craneStatus == -1)
        {
            _BGMPlayer.StopBGM(0);
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
                isExecuted[12] = false;
                creditSystem.AddCreditPlayed();
                probability = creditSystem.ProbabilityCheck();
                Debug.Log("Probability:" + probability);
                _SEPlayer.PlaySE(1, 1);
                await Task.Delay(3000);
                timer.StartTimer();
                _BGMPlayer.PlayBGM(1);
                if (craneStatus == 1) craneStatus = 2;
            }

            //開始準備;
        }

        if (craneStatus == 2)
        { //移動可能
            if (timer.limitTimeNow <= 10)
            {
                if (!isExecuted[craneStatus])
                {
                    isExecuted[craneStatus] = true;
                    _BGMPlayer.StopBGM(1);
                    _SEPlayer.PlaySE(2, 1);
                }
                if (!_SEPlayer._AudioSource[2].isPlaying && timer.limitTimeNow <= 9) _SEPlayer.PlaySE(3, 2147483647);
            }
            if (timer.limitTimeNow == 0) craneStatus = 7;
            InputKeyCheck();
            InputLeverCheck();
        }

        if (craneStatus == 7)
        {
            if (!isExecuted[craneStatus])
            {
                isExecuted[craneStatus] = true;
                _SEPlayer.StopSE(3);
                _SEPlayer.PlaySE(5, 1);
                creditSystem.ResetNowPayment();
                craneBox.rightMoveFlag = false;
                craneBox.leftMoveFlag = false;
                craneBox.backMoveFlag = false;
                craneBox.forwardMoveFlag = false;
                ropeManager.ArmUnitDownForceStop();
                ropeManager.ArmUnitUpForceStop();
                leverState = 0;

                if (probability) armPower = catchArmpowersuccess;
                else armPower = catchArmpower;
                armController.MotorPower(armPower);
                if (armState == 1)
                {
                    armState = 0;
                    armController.ArmClose();
                }
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
            if (ropeManager.UpFinished() && craneStatus == 8) craneStatus = 9;
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
                timer.limitTimeNow = limitTimeSet;
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
        if (!_SEPlayer._AudioSource[4].isPlaying)
            _SEPlayer.PlaySE(4, 1);
        probability = creditSystem.ProbabilityCheck();
        Debug.Log("Probability:" + probability);
    }

    public void InputLeverCheck() // キーボード，UI共通のレバー処理
    {
        if (host.playable)
        {
            if (Input.GetKey(KeyCode.H) || lever[0].rightFlag)
                craneBox.rightMoveFlag = true;
            else if (Input.GetKeyUp(KeyCode.H) || !lever[0].rightFlag)
                craneBox.rightMoveFlag = false;
            if (Input.GetKey(KeyCode.F) || lever[0].leftFlag)
                craneBox.leftMoveFlag = true;
            else if (Input.GetKeyUp(KeyCode.F) || !lever[0].leftFlag)
                craneBox.leftMoveFlag = false;
            if (Input.GetKey(KeyCode.T) || lever[0].backFlag)
                craneBox.backMoveFlag = true;
            else if (Input.GetKeyUp(KeyCode.T) || !lever[0].backFlag)
                craneBox.backMoveFlag = false;
            if (Input.GetKey(KeyCode.G) || lever[0].forwardFlag)
                craneBox.forwardMoveFlag = true;
            else if (Input.GetKeyUp(KeyCode.G) || !lever[0].forwardFlag)
                craneBox.forwardMoveFlag = false;

            if ((Input.GetKeyDown(KeyCode.I) || lever[1].backFlag) && leverState != 2)
            {
                Debug.Log("Up");
                leverState = 2;
                ropeManager.ArmUnitUp();
            }
            if ((Input.GetKeyDown(KeyCode.K) || lever[1].forwardFlag) && leverState != 1 && !support.isShieldcollis)
            {
                Debug.Log("Down");
                leverState = 1;
                ropeManager.ArmUnitDown();
            }
            if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && !lever[1].backFlag && !lever[1].forwardFlag)
            {
                leverState = 0;
                ropeManager.ArmUnitUpForceStop();
                ropeManager.ArmUnitDownForceStop();
            }
            if (support.isShieldcollis) ropeManager.ArmUnitDownForceStop();
        }
    }

    public void InputKeyCheck()
    {
        if (host.playable)
        {
            if (Input.GetKeyDown(KeyCode.O) && armState == 0)
            {
                armState = 1;
                armController.ArmOpen();
            }
            if (Input.GetKeyDown(KeyCode.L) && armState == 1)
            {
                armState = 0;
                armController.ArmClose();
            }
        }
    }

    public void ButtonDown(int num)
    {
        if (host.playable)
        {
            if (craneStatus >= 2 && craneStatus <= 3)
            {
                if (num == 0 && armState == 0)
                {
                    armState = 1;
                    armController.ArmOpen();
                }
                if (num == 1 && armState == 1)
                {
                    armState = 0;
                    armController.ArmClose();
                }
            }
        }
    }
    public void InsertCoin()
    {
        if (host.playable) creditSystem.GetPayment(100);
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
