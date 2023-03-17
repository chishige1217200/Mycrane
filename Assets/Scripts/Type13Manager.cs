using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Type13Manager : CraneManager
{
    public Type13Player[] player = new Type13Player[2];
    [SerializeField] bool player2 = false; //player2の場合true
    public Animator[] animator = new Animator[2];
    [SerializeField] Text Credit;
    [SerializeField] TextMesh credit3d;
    [SerializeField] GameObject[] LCD = new GameObject[2];
    private int playingBooth = -1;
    // Start is called before the first frame update
    void Start()
    {
        craneStatus = -2;
        craneType = 13;

        canvas = transform.Find("Canvas").gameObject;
        //creditSystem = GetComponent<CreditSystem>();
        //sp = transform.Find("SE").GetComponent<SEPlayer>();
        getPoint = transform.Find("GetPoint").GetComponent<GetPoint>();
        //host = transform.root.Find("Glass").GetComponent<MachineHost>();


        // クレジット情報登録
        creditSystem.rateSet[0, 0] = priceSet[0];
        creditSystem.rateSet[1, 0] = priceSet[1];
        creditSystem.rateSet[0, 1] = timesSet[0];
        creditSystem.rateSet[1, 1] = timesSet[1];
        if (isHibernate)
        {
            credit3d.text = "---";
            creditSystem.SetHibernate();
        }

        creditSystem.SetSEPlayer(sp);
        creditSystem.SetCreditSound(0);
        getPoint.SetManager(this);
        LCD[0].SetActive(true);
        getSoundNum = 6;
        host.manualCode = 17;
    }

    // Update is called once per frame
    void Update()
    {
        if (useUI && host.playable && !canvas.activeSelf) canvas.SetActive(true);
        else if (!host.playable && canvas.activeSelf) canvas.SetActive(false);

        if (!player2 && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();
        else if (player2 && (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus))) InsertCoin();

        if (craneStatus == 1)
        {
            DetectKey(1);
        }
    }

    public override void InsertCoin()
    {
        if (!isHibernate && host.playable && player[0].craneStatus >= 0 && player[1].craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);

            if (credit < 100) credit3d.text = credit.ToString("D1");
            else credit3d.text = "99.";

            if (credit > 0 && craneStatus == 0)
            {
                craneStatus = 1;
                LCD[0].SetActive(false);
                LCD[1].SetActive(true);
            }
        }
    }

    public override void InsertCoinAuto()
    {
        if (!isHibernate && player[0].craneStatus >= 0 && player[1].craneStatus >= 0)
        {
            int credit = creditSystem.Pay(100);

            if (credit < 100) credit3d.text = credit.ToString("D1");
            else credit3d.text = "99.";

            if (credit > 0 && craneStatus == 0)
            {
                craneStatus = 1;
                LCD[0].SetActive(false);
                LCD[1].SetActive(true);
            }
        }
    }

    public void PlayerStart()
    {
        creditSystem.PlayStart();
    }

    public void PlayerEnd()
    {
        int credit = creditSystem.Pay(0);

        if (credit < 100) credit3d.text = credit.ToString("D1");
        else credit3d.text = "99.";

        if (credit > 0) craneStatus = 1;
        else
        {
            craneStatus = 0;
            LCD[1].SetActive(false);
            LCD[0].SetActive(true);
        }
    }

    public void SelectBooth(int id)
    {
        if (host.playable)
        {
            player[id].GameStart();
            playingBooth = id;
            craneStatus = 2;
        }
    }

    public void ResetPayment()
    {
        creditSystem.ResetPayment();
    }

    public override void GetPrize()
    {
        for (int i = 0; i < 2; i++) animator[i].SetTrigger("GetPrize");
        base.GetPrize();
        player[playingBooth].GetPrize();
    }

    protected override void DetectKey(int num)
    {
        if (num == 1)
        {
            if ((!player2 && Input.GetKeyDown(KeyCode.R)) || (player2 && Input.GetKeyDown(KeyCode.O))) SelectBooth(0);
            if ((!player2 && Input.GetKeyDown(KeyCode.V)) || (player2 && Input.GetKeyDown(KeyCode.Period))) SelectBooth(1);
        }
    }

    public override void ButtonDown(int num)
    {
        if (craneStatus == 1)
            SelectBooth(num);
    }

    public override void ButtonUp(int num)
    {
    }

    public void CreditSegUpdate()
    {
        int credit = creditSystem.Pay(0);

        if (credit < 100) credit3d.text = credit.ToString("D1");
        else credit3d.text = "99.";

        if (credit < 100) Credit.text = credit.ToString("D2");
        else Credit.text = "99.";
    }
}
