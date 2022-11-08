using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPrizeV2 : MonoBehaviour
{
    [SerializeField] GameObject targetObjects;
    private Transform targetParent;
    private GameObject instance;
    private CraneManagerV2 c;
    private MachineHost host;
    [SerializeField] bool player2 = false;
    // Start is called before the first frame update
    void Start()
    {
        c = this.GetComponent<CraneManagerV2>();
        host = c.host;
        if (c == null || targetObjects == null) Debug.LogError("エラー: ResetPrize");
        if (targetObjects.activeSelf) targetObjects.SetActive(false);
        Invoke("Init", 1f);
    }

    void Init()
    {
        switch (c.GetCType())
        {
            case 2:
            case 3:
            case 7:
            case 8:
                targetParent = transform;
                break;
            default: // 2人用筐体の場合
                targetParent = transform.parent;
                break;
        }

        instance = Instantiate(targetObjects, targetParent);
        instance.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (host.playable)
        {
            if (!player2 && (Input.GetKeyDown(KeyCode.Quote) || Input.GetKeyDown(KeyCode.KeypadDivide))) Reset();
            else if (player2 && (Input.GetKeyDown(KeyCode.Backslash) || Input.GetKeyDown(KeyCode.KeypadMultiply))) Reset();
        }
    }

    public void Reset()
    {
        if ((c.GetCType() != 8 && (c.craneStatus == 0 || c.craneStatus == 1)) || (c.GetCType() == 8 && (c.craneStatus >= 0 && c.craneStatus <= 2)))
        {
            if (instance != null) DestroyImmediate(instance);
            instance = Instantiate(targetObjects, targetParent);
            instance.SetActive(true);
        }
    }
}
