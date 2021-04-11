using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Type4Manager : MonoBehaviour
{
    public int craneStatus = -1; //-1:初期化動作，0:待機状態
    float leftCatchArmpower = 100f; //左アームパワー
    float rightCatchArmpower = 100f; //右アームパワー
    float armApertures = 100f; //開口率
    int operationType = 1; //0:ボタン式，1:レバー式
    int soundType = 0; //BGMの切り替え．0・1
    int catchTime = 2000; //キャッチに要する時間(m秒)
    private bool[] instanceFlag = new bool[17]; //各craneStatusで1度しか実行しない処理の管理
    public bool buttonFlag = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] bool player2 = false; //player2の場合true
    [SerializeField] bool playable = true; //playableがtrueのとき操作可能
    [SerializeField] bool button3 = true; //button3の使用可否
    private Vector2 startPoint; // 開始位置座標定義
    private Vector2 homePoint; // 獲得口座標定義（prizezoneTypeが9のとき使用）
    private Vector2 vec2offset = new Vector2(0.05f, 0.1f); // <=0.5, <=0.6 座標設定用Temp
    public int prizezoneType = 9; // 1:左手前，2：左奥，3：右手前，4：右奥，5：左，6：手前，7：右，8：奥，9：特定座標（1P時）2Pは左右反転
    Vector2 craneHost; //クレーンゲームの中心位置定義
    CreditSystem creditSystem; //クレジットシステムのインスタンスを格納（以下同）
    SEPlayer _SEPlayer;
    //Type4ArmController _ArmController;
    Transform temp;
    CraneBox _CraneBox;
    GetPoint _GetPoint;
    RopeManager _RopeManager;
    ArmControllerSupport support;
    ArmNail[] nail = new ArmNail[2];

    //For test-----------------------------------------

    public Text craneStatusdisplayed;

    //-------------------------------------------------

    async void Start()
    {
        creditSystem = this.transform.Find("CreditSystem").GetComponent<CreditSystem>();
        _SEPlayer = this.transform.Find("SE").GetComponent<SEPlayer>();
        _GetPoint = this.transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
        temp = this.transform.parent;
        craneHost = new Vector2(temp.position.x, temp.position.z);
        temp = this.transform.Find("CraneUnit").transform;

        // ロープとアームコントローラに関する処理
        _RopeManager = this.transform.Find("RopeManager").GetComponent<RopeManager>();
        //_ArmController = temp.Find("ArmUnit").GetComponent<Type1ArmController>();
        support = temp.Find("ArmUnit").Find("Main").GetComponent<ArmControllerSupport>();
        //nail[0] = temp.Find("ArmUnit").Find("Arm1").GetComponent<ArmNail>();
        //nail[1] = temp.Find("ArmUnit").Find("Arm2").GetComponent<ArmNail>();

        // CraneBoxに関する処理
        _CraneBox = temp.Find("CraneBox").GetComponent<CraneBox>();
        _CraneBox.GetManager(4);

        // ロープにマネージャー情報をセット
        _RopeManager.SetManagerToPoint(4);
        creditSystem.GetSEPlayer(_SEPlayer);
        creditSystem.playable = playable;
        _GetPoint.GetManager(4);
        _RopeManager.ArmUnitUp();
        creditSystem.SetCreditSound(0);
        creditSystem.GetSEPlayer(_SEPlayer);
        support.GetManager(4);
        support.GetRopeManager(_RopeManager);
        support.pushTime = 300; // 押し込みパワーの調整
        /*for (int i = 0; i < 2; i++)
        {
            nail[i].GetManager(4);
            nail[i].GetRopeManager(_RopeManager);
        }*/

        for (int i = 0; i < 15; i++)
            instanceFlag[i] = false;

        if (!button3) this.transform.Find("Canvas").Find("ControlGroup").Find("Button 3").gameObject.SetActive(false);

        // イニシャル移動とinsertFlagを後に実行
        await Task.Delay(3000);
        //_ArmController.ArmLimit(armApertures);
        if (!player2) _CraneBox.leftMoveFlag = true;
        else _CraneBox.rightMoveFlag = true;
        _CraneBox.forwardMoveFlag = true;

        if (!player2)
        {
            startPoint = new Vector2(-0.65f + vec2offset.x + craneHost.x, -0.3f + vec2offset.y + craneHost.y);
            homePoint = new Vector2(-0.65f + vec2offset.x + craneHost.x, -0.3f + vec2offset.y + craneHost.y);
        }
        else
        {
            startPoint = new Vector2(0.65f - vec2offset.x + craneHost.x, -0.3f + vec2offset.y + craneHost.y);
            homePoint = new Vector2(0.65f - vec2offset.x + craneHost.x, -0.3f + vec2offset.y + craneHost.y);
        }

        while (true)
        {
            if (!player2 && _CraneBox.CheckPos(1))
            {
                _CraneBox.goPoint = startPoint;
                _CraneBox.goPositionFlag = true;
                break;
            }
            if (player2 && _CraneBox.CheckPos(3))
            {
                _CraneBox.goPoint = startPoint;
                _CraneBox.goPositionFlag = true;
                break;
            }
            await Task.Delay(1000);
        }

        await Task.Delay(500);

        while (true)
        {
            if (_CraneBox.CheckPos(9))
            {
                creditSystem.insertFlag = true;
                craneStatus = 0;
                break;
            }
            await Task.Delay(1000);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
