using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class BoothData
{
    [SerializeField] public int craneType = 0;
    [SerializeField] CraneManagerV3[] manager;
}

public class JsonManager : MonoBehaviour
{
    string dirPath;

    void Start()
    {
        // Get the path of the Game data folder
        dirPath = Application.persistentDataPath;
        dirPath += @"/EditModeData";

        // Output the Game data path to the console
        Debug.Log("dataPath : " + dirPath);

        if (!Directory.Exists(dirPath))
        {
            Debug.Log("フォルダが存在しません．作成します．");
            Directory.CreateDirectory(dirPath);
        }

        GetDataList();


    }

    public void OpenDir(){
        if (Directory.Exists(dirPath))
            Application.OpenURL("file://" + dirPath);
    }

    void GetDataList() // Jsonデータリストを取得
    {
        try
        {
            string[] files = Directory.GetFiles(dirPath, "*.json", SearchOption.AllDirectories);
            foreach (string name in files)
            {
                Debug.Log(name);
                GetJson(name);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void GetJson(string jsonPath)
    {
        string inputString = File.ReadAllText(jsonPath);
        BoothData bd = JsonUtility.FromJson<BoothData>(inputString);
        Debug.Log(bd.craneType);
    }
}
