using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type3Manager : MonoBehaviour
{
    public CreditSystem creditSystem; //クレジットシステムのインスタンスを格納
    int craneStatus = -1; //-1:初期化動作，0:待機状態
    double catchArmpower; //掴むときのアームパワー(%，未確率時)
    double upArmpower; //上昇時のアームパワー(%，未確率時)
    double backArmpower; //獲得口移動時のアームパワー(%，未確率時)
    double catchArmpowersuccess; //同確率時
    double upArmpowersuccess; //同確率時
    double backArmpowersuccess; //同確率時
    int soundType = 1; //0:CARINO 1:CARINO4 2:BAMBINO 3:neomini
    bool resetFlag = false; //投入金額リセットは1プレイにつき1度のみ実行
    private BGMPlayer _BGMPlayer;
    private SEPlayer _SEPlayer;
    private RopePoint[] _RopePoint;
    private Type3ArmController _ArmController;
    private Type3CraneUnitMover _CraneUnitMover;
    Transform temp;
    GameObject craneBox;
    CraneBox _CraneBox;
    //GameObject craneBoxSupport;
    public float moveSpeed = 0.1f;
    //public bool rightMoveFlag = false;
    //public bool leftMoveFlag = false;
    //public bool backMoveFlag = false;
    //public bool forwardMoveFlag = false;
    private GameObject ropeHost;
    //Transform temp;

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    void Start()
    {
        // 様々なコンポーネントの取得
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        temp = this.transform.Find("CraneUnit").transform;
        // ロープとアームコントローラに関する処理
        // _CraneUnitMover = temp.GetComponent<Type3CraneUnitMover>();
        _RopePoint = new RopePoint[8];
        _RopePoint[0] = temp.Find("Rope").Find("Sphere (1)").GetComponent<RopePoint>();
        _RopePoint[1] = temp.Find("Rope").Find("Sphere (2)").GetComponent<RopePoint>();
        _RopePoint[2] = temp.Find("Rope").Find("Sphere (3)").GetComponent<RopePoint>();
        _RopePoint[3] = temp.Find("Rope").Find("Sphere (4)").GetComponent<RopePoint>();
        _RopePoint[4] = temp.Find("Rope").Find("Sphere (5)").GetComponent<RopePoint>();
        _RopePoint[5] = temp.Find("Rope").Find("Sphere (6)").GetComponent<RopePoint>();
        _RopePoint[6] = temp.Find("Rope").Find("Sphere (7)").GetComponent<RopePoint>();
        _RopePoint[7] = temp.Find("Rope").Find("Sphere (8)").GetComponent<RopePoint>();
        _ArmController = temp.Find("ArmUnit").GetComponent<Type3ArmController>();

        // CraneBoxに関する処理
        craneBox = temp.Find("CraneBox").gameObject;
        _CraneBox = craneBox.GetComponent<CraneBox>();
        //craneBoxSupport = temp.Find("CraneBoxSupport").gameObject;
        ropeHost = temp.Find("Rope").gameObject;

        if (soundType == 0) creditSystem.SetCreditSound(0);
        if (soundType == 1) creditSystem.SetCreditSound(6);
        if (soundType == 2) creditSystem.SetCreditSound(13);
        if (soundType == 3) creditSystem.SetCreditSound(-1);
    }

    async void Update()
    {
        craneStatusdisplayed.text = craneStatus.ToString();
        if (craneStatus == -2)
        {
            //_CraneUnitMover.leftMoveFlag = true;
            //_CraneUnitMover.forwardMoveFlag = true;
        }
        if (craneStatus == -1)
        {
            _BGMPlayer.StopBGM(soundType);
            //コイン投入無効化;
        }

        if (craneStatus == 0)
        {
            //コイン投入有効化;
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
            switch (soundType)
            {
                case 0:
                    _BGMPlayer.PlayBGM(0);
                    break;
                case 1:
                    _BGMPlayer.PlayBGM(1);
                    break;
                case 2:
                    _BGMPlayer.PlayBGM(2);
                    break;
                case 3:
                    _BGMPlayer.StopBGM(4);
                    _BGMPlayer.PlayBGM(3);
                    break;
            }
        }

        if (craneStatus == 1)
        {
            //コイン投入有効化;
            _BGMPlayer.StopBGM(soundType);
            switch (soundType)
            {
                case 1:
                    //await Task.Delay(1000);
                    _SEPlayer.PlaySE(7, 2147483647);
                    break;
                case 3:
                    _BGMPlayer.PlayBGM(4);
                    break;
            }
            //右移動ボタン有効化;
        }

        if (craneStatus == 2)
        { //右移動中
            //_CraneUnitMover.rightMoveFlag = true;
            //コイン投入無効化;
            if (resetFlag == false)
            {
                resetFlag = true;
                creditSystem.ResetNowPayment();
            }
            //クレーン右移動;
            switch (soundType)
            {
                case 0:
                    _SEPlayer.PlaySE(1, 2147483647);
                    break;
                case 1:
                    _SEPlayer.StopSE(7);
                    _SEPlayer.PlaySE(8, 2147483647);
                    break;
                case 2:
                    _SEPlayer.PlaySE(14, 2147483647);
                    break;
                case 3:
                    _SEPlayer.PlaySE(18, 2147483647);
                    break;
            }
            //右移動効果音ループ再生;
        }

        if (craneStatus == 3)
        {
            //_CraneUnitMover.rightMoveFlag = false;
            switch (soundType)
            {
                case 2:
                    _SEPlayer.StopSE(14);
                    break;
                case 3:
                    _SEPlayer.StopSE(18);
                    break;
            }
            //右移動効果音ループ再生停止;
            //奥移動ボタン有効化;
        }

        if (craneStatus == 4)
        { //奥移動中
            //_CraneUnitMover.backMoveFlag = true;
            //クレーン奥移動;
            switch (soundType)
            {
                case 0:
                    _SEPlayer.StopSE(1);
                    _SEPlayer.PlaySE(2, 2147483647);
                    break;
                case 1:
                    _SEPlayer.StopSE(8);
                    _SEPlayer.PlaySE(9, 2147483647);
                    break;
                case 2:
                    _SEPlayer.PlaySE(14, 2147483647);
                    break;
                case 3:
                    _SEPlayer.PlaySE(19, 2147483647);
                    break;
            }
            //奥移動効果音ループ再生;
        }

        if (craneStatus == 5)
        {
            //_CraneUnitMover.backMoveFlag = false;
            switch (soundType)
            {
                case 0:
                    _SEPlayer.StopSE(2);
                    _SEPlayer.PlaySE(3, 2147483647);
                    break;
                case 1:
                    _SEPlayer.StopSE(9);
                    _SEPlayer.PlaySE(10, 2147483647);
                    break;
                case 2:
                    _SEPlayer.StopSE(14);
                    break;
                case 3:
                    _SEPlayer.StopSE(19);
                    _SEPlayer.PlaySE(20, 1);
                    break;
            }
            _ArmController.motor_on();
            //奥移動効果音ループ再生停止;
            //アーム開く音再生;
            //アーム開く;
        }

        if (craneStatus == 6)
        {
            switch (soundType)
            {
                case 2:
                    _SEPlayer.PlaySE(15, 2147483647);
                    break;
                case 3:
                    _SEPlayer.PlaySE(21, 2147483647);
                    break;
            }
            //アーム下降音再生
            //アーム下降;
        }

        if (craneStatus == 7)
        {
            switch (soundType)
            {
                case 0:
                    _SEPlayer.StopSE(3);
                    _SEPlayer.PlaySE(4, 2147483647);
                    break;
                case 1:
                    _SEPlayer.StopSE(10);
                    _SEPlayer.PlaySE(11, 2147483647);
                    break;
                case 3:
                    _SEPlayer.StopSE(21);
                    break;
            }
            _ArmController.motor_off();
            //アーム下降音再生停止;
            //アーム掴む音再生;
            //アーム掴む;
        }

        if (craneStatus == 8)
        {
            switch (soundType)
            {
                case 3:
                    _SEPlayer.PlaySE(22, 2147483647);
                    break;
            }
            //アーム上昇音再生;
            //アーム上昇;
        }

        if (craneStatus == 9)
        {
            switch (soundType)
            {
                case 3:
                    _SEPlayer.StopSE(22);
                    break;
            }
            //アーム上昇停止音再生;
            //アーム上昇停止;
        }

        if (craneStatus == 10)
        {
            switch (soundType)
            {
                case 0:
                    _SEPlayer.StopSE(4);
                    _SEPlayer.PlaySE(1, 2147483647);
                    break;
                case 2:
                    _SEPlayer.StopSE(15);
                    _SEPlayer.PlaySE(14, 2147483647);
                    break;
                case 3:
                    _SEPlayer.PlaySE(23, 2147483647);
                    break;
            }
            //アーム獲得口ポジション移動音再生;
            //_CraneUnitMover.leftMoveFlag = true;
            //_CraneUnitMover.forwardMoveFlag = true;
            //アーム獲得口ポジションへ;
        }

        if (craneStatus == 11)
        {
            //_CraneUnitMover.leftMoveFlag = false;
            //_CraneUnitMover.forwardMoveFlag = false;
            switch (soundType)
            {
                case 0:
                    _SEPlayer.StopSE(1);
                    break;
                case 1:
                    _SEPlayer.StopSE(11);
                    break;
                case 2:
                    _SEPlayer.StopSE(14);
                    _SEPlayer.PlaySE(17, 1);
                    break;
                case 3:
                    _SEPlayer.StopSE(23);
                    _SEPlayer.PlaySE(24, 1);
                    break;
            }
            _ArmController.motor_on();
            //アーム開く音再生;
            //アーム開く;
            //1秒待機;
        }

        if (craneStatus == 12)
        {
            switch (soundType)
            {
                case 3:
                    _SEPlayer.PlaySE(25, 1);
                    break;
            }
            _ArmController.motor_off();
            //アーム閉じる音再生;
            //アーム閉じる;
            //1秒待機;
            resetFlag = false;
            if (creditSystem.creditDisplayed > 0)
                craneStatus = 1;
            else
                craneStatus = 0;
        }
    }

    async public void ArmUnitDown()
    {
        int i = 7;
        while (true)
        {
            _RopePoint[i].moveDownFlag = true;
            while (true)
            {
                if (!_RopePoint[i].moveDownFlag)
                {
                    if (i > 0)
                    {
                        i--;
                        _RopePoint[i].moveDownFlag = true;
                        break;
                    }
                    else
                    {
                        ArmUnitDownForceStop();
                        return;
                    }
                }
                await Task.Delay(1);
            }
        }
    }

    public void ArmUnitDownForceStop()
    {
        for (int i = 0; i <= 7; i++)
        {
            _RopePoint[i].moveDownFlag = false;
        }
    }

    public void ArmUnitUp()
    {
        for (int i = 0; i <= 7; i++)
        {
            _RopePoint[i].moveUpFlag = true;
        }
    }

    void RightMove()
    {
        craneBox.transform.position += new Vector3(moveSpeed, 0, 0);
        ropeHost.transform.position += new Vector3(moveSpeed, 0, 0);
    }

    void LeftMove()
    {
        craneBox.transform.position -= new Vector3(moveSpeed, 0, 0);
        ropeHost.transform.position -= new Vector3(moveSpeed, 0, 0);
    }

    void BackMove()
    {
        craneBox.transform.position += new Vector3(0, 0, moveSpeed);
        ropeHost.transform.position += new Vector3(0, 0, moveSpeed);
        //craneBoxSupport.transform.position += new Vector3(0, 0, moveSpeed);
    }

    void ForwardMove()
    {
        craneBox.transform.position -= new Vector3(0, 0, moveSpeed);
        ropeHost.transform.position -= new Vector3(0, 0, moveSpeed);
        //craneBoxSupport.transform.position -= new Vector3(0, 0, moveSpeed);
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