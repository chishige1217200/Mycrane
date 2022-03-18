using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class ResetPrize : MonoBehaviour
{
    [SerializeField] GameObject targetObjects;
    private Transform targetParent;
    private GameObject instance;
    private CraneManager c;
    private MachineHost host;
    [SerializeField] bool player2 = false;
    // Start is called before the first frame update
    async void Start()
    {
        c = this.GetComponent<CraneManager>();
        host = c.host;
        if (c == null || targetObjects == null) Debug.LogError("エラー: ResetPrize");
        if (targetObjects.activeSelf) targetObjects.SetActive(false);

        await Task.Delay(1000);

        switch (c.GetCType())
        {
            case 2:
            case 3:
            case 7:
            case 8:
                targetParent = transform;
                break;
            default:
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
        if ((c.GetCType() != 8 && (c.GetStatus() == 0 || c.GetStatus() == 1)) || (c.GetCType() == 8 && (c.GetStatus() >= 0 && c.GetStatus() <= 2)))
        {
            if (instance != null) DestroyImmediate(instance);
            instance = Instantiate(targetObjects, targetParent);
            instance.SetActive(true);
        }
    }
}
