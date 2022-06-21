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
    bool[] isExecuted = new bool[18]; //各craneStatusで1度しか実行しない処理の管理
    public bool buttonPushed = false; //trueならボタンをクリックしているかキーボードを押下している
    [SerializeField] bool player2 = false; //player2の場合true
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
        //getPoint = transform.Find("Floor").Find("GetPoint").GetComponent<GetPoint>();
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

        // ロープにマネージャー情報をセット
        creditSystem.SetSEPlayer(sp);
        //getPoint.SetManager(this);
        creditSystem.SetCreditSound(0);

        for (int i = 0; i < isExecuted.Length; i++)
            isExecuted[i] = false;

        craneStatus = -1;
    }

    // Update is called once per frame
    void Update()
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
            else if (craneStatus == 4)
            {
                if (!player2) craneBox.Right();
                else craneBox.Left();
            }
            else if (craneStatus == 6)
            {
                // 伸ばす
            }
            else if (craneStatus == 8 || craneStatus == 16)

                craneBox.Down();
            else if (craneStatus == 10)
            {
                // 縮む
            }
            else if (craneStatus == 12)
                craneBox.Up();
            else if (craneStatus == 14)
            {
                if (!player2) craneBox.Left();
                else craneBox.Right();
            }
        }
    }

    protected override void DetectKey(int num)
    {
    }

    public override void ButtonDown(int num)
    {
    }

    public void ButtonUp(int num)
    {
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
