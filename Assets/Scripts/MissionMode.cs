using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MissionMode : MonoBehaviour
{
    [SerializeField] GameObject t;
    [SerializeField] CraneManager[] c = new CraneManager[2];
    [SerializeField] int limitCost = 1000;
    CreditSystem[] creditSystem = new CreditSystem[2];
    GameObject missionPanel;
    GameObject gameOverPanel;
    GameObject gameClearPanel;
    bool gameClear = false;
    bool isExecuted = false;
    int playerCount = 1;
    void Start()
    {
        //credit = this.transform.Find("Canvas").Find("Credit").GetComponent<Text>();
        missionPanel = this.transform.Find("Canvas").Find("MissionPanel").gameObject;
        gameOverPanel = this.transform.Find("Canvas").Find("GameOverPanel").gameObject;
        gameClearPanel = this.transform.Find("Canvas").Find("GameClearPanel").gameObject;
        if (c[0] == null) Debug.LogError("Mission: 基準のCraneManagerがセットされていません");
        if (t == null) Debug.LogError("Mission: 基準のGameObjectがセットされていません");
        switch (c[0].GetCType())
        {
            case 1:
            case 4 - 6:
            case 10:
                playerCount = 2;
                c[1] = t.transform.Find("2P").GetComponent<CraneManager>();
                creditSystem[0] = t.transform.Find("1P").Find("CreditSystem").GetComponent<CreditSystem>();
                creditSystem[1] = t.transform.Find("2P").Find("CreditSystem").GetComponent<CreditSystem>();
                break;
            case 9:
                creditSystem[0] = t.transform.Find("1P").Find("CreditSystem").GetComponent<CreditSystem>();
                break;
            default:
                creditSystem[0] = t.transform.Find("CreditSystem").GetComponent<CreditSystem>();
                break;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || (playerCount == 1 && (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))) || (playerCount == 2 && (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadPeriod))))
        {
            if (playerCount == 1 && creditSystem[0].nowpaidSum >= limitCost)
            {
                c[0].isHibernate = true;
            }
            else if (playerCount == 2 && creditSystem[0].nowpaidSum + creditSystem[1].nowpaidSum >= limitCost)
            {
                c[0].isHibernate = true;
                c[1].isHibernate = true;
            }
        }

        if (playerCount == 1 && c[0].isHibernate && c[0].GetStatus() == 0 && creditSystem[0].Pay(0) == 0)
        {
            if (!isExecuted)
            {
                isExecuted = true;
                GameOver();
            }
            // 数秒待ってゲームオーバー
        }
        else if (playerCount == 2 && c[0].isHibernate && c[1].isHibernate && c[0].GetStatus() == 0 && c[1].GetStatus() == 0 && creditSystem[0].Pay(0) == 0 && creditSystem[1].Pay(0) == 0)
        {
            if (!isExecuted)
            {
                isExecuted = true;
                GameOver();
            }
            // 数秒待ってゲームオーバー
        }
    }

    public void GameClear()
    {
        gameClear = true;
        gameOverPanel.SetActive(false);
        gameClearPanel.SetActive(true);
        // ゲームクリア画面の表示
    }

    async void GameOver()
    {
        await Task.Delay(3000);
        if (!gameClear)
        {
            gameOverPanel.SetActive(true);
        }
        // ゲームオーバー画面の表示
    }

    public void CloseMissionPanel()
    {
        missionPanel.SetActive(false);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // シーンのリセット
    }
}
