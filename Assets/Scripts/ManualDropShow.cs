using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualDropShow : MonoBehaviour
{
    //Dropdownを格納する変数
    [SerializeField] private Dropdown dropdown;
    //表示候補を格納する変数
    [SerializeField] private GameObject[] panel = new GameObject[2];

    // オプションが変更されたときに実行するメソッド
    public void ChangeDropDown()
    {
        for (int i = 0; i < panel.Length; i++)
        {
            panel[i].SetActive(false);
        }

        if (dropdown.value >= 1 && dropdown.value <= panel.Length)
            panel[dropdown.value - 1].SetActive(true);
    }
}
