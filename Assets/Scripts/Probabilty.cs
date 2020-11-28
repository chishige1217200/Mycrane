using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probabilty : MonoBehaviour
{
    public int mode = 0; // 0：確率なし，1:値段天井設定，2:値段周期設定，3:クレジット回数天井設定，4:クレジット回数周期設定，5:ランダム確率
    public bool probabiltyFlag = false; // trueのとき確率
    int n = 3;

    // Start is called before the first frame update
    void Start()
    {
        if(mode == 0) probabiltyFlag = true;
    }

    // Update is called once per frame
    public bool ProbabiltyCheck()
    {
        if(mode == 1);
        if(mode == 2);
        if(mode == 3);
        if(mode == 4);
        if(mode == 5)
            if(Random.Range(1, n + 1) == 0) return true; // 1/nの確率

        return false;
    }
}
