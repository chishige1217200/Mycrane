using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type13Manager : MonoBehaviour
{
    Type13Player[] player = new Type13Player[2];
    [SerializeField] int[] priceSet = new int[2];
    [SerializeField] int[] timesSet = new int[2];
    protected int getSoundNum = -1;
    [SerializeField] bool player2 = false; //player2の場合true
    protected CreditSystem creditSystem;
    public SEPlayer sp;
    protected GetPoint getPoint;
    [SerializeField] MachineHost host;
    protected GameObject canvas;
    [SerializeField] TextMesh credit3d;
    public bool isHibernate = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!player2 && (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))) InsertCoin();
        else if (player2 && (Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Minus))) InsertCoin();
    }

    public int isAdvertise()
    {
        if (creditSystem.Pay(0) == 0)
        {
            if (player[0].craneStatus == 0 && player[1].craneStatus == 0) return 1;
            else if (player[0].craneStatus < 0 || player[1].craneStatus < 0) return -1;
        }
        return 0;
    }

    public void InsertCoin()
    {
        if (!isHibernate && host.playable && isAdvertise() != -1)
        {
            int credit = creditSystem.Pay(100);

            if (credit < 100) credit3d.text = credit.ToString("D2");
            else credit3d.text = "99.";
        }
    }
}
