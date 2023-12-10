using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Type5ManagerConfig : MonoBehaviour
{
    public float[] armLPowerConfig = new float[3]; // アームパワーL(%，未確率時)
    public float[] armLPowerConfigSuccess = new float[3]; // アームパワーL(%，確率時)
    public float[] armRPowerConfig = new float[3]; // アームパワーR(%，未確率時)
    public float[] armRPowerConfigSuccess = new float[3]; // アームパワーR(%，確率時)
    public float armApertures = 80f; // 開口率
    public int[] armSize = new int[2]; // 0:なし，1:S，2:M，3:L
    public float armUnitAngle = 0f; // -90 to 90
    public float[] boxRestrictions = new float[2];
    public float downRestriction = 100f;
    public bool downStop = true; // button3の使用可否
    public int pushTime = 300; // 押し込み時間調整（ミリ秒）
    public Vector3 startPoint = new Vector3(); // 開始位置座標定義（x, y, z）
    public Vector3 homePoint = new Vector3(); // 獲得口座標定義（prizezoneTypeが9のとき使用．x, z）
    public int prizezoneType = 9; // 1:左手前，2：左奥，3：右手前，4：右奥，5：左，6：手前，7：右，8：奥，9：特定座標
    public bool doRelease = true; // 離し位置でアームを開く
    public bool alytMode = false; // 時間内取り放題モード設定
    public int limitTimeSet = 60; // 残り時間を設定（時間内取り放題モード）
}
