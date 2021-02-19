using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    public Canvas canvas; // Canvasの変数
    public RectTransform canvasRect; // キャンバス内のレクトトランスフォーム
    public Vector2 MousePos; // マウスの位置の最終的な格納先
    public RectTransform pointer; // 自身のゲームオブジェクトのRectTransform
    public bool rightFlag = false; // 右に倒しているとき
    public bool leftFlag = false; // 左に倒しているとき
    public bool backFlag = false; // 奥に倒しているとき
    public bool forwardFlag = false; // 手前に倒しているとき
    bool isClicked = false;
    Vector2 init; // レバーの基準初期座標
    float leverRange = 30f; // レバーの可動域
    float radian = 0f; // 角度計算用

    void Start()
    {
        // canvas内にあるRectTransformをcanvasRectに入れる
        canvasRect = canvas.GetComponent<RectTransform>();
        // 自身のゲームオブジェクトのRectTransformをpointerに入れる
        pointer = this.GetComponent<RectTransform>();
        init = new Vector2(pointer.anchoredPosition.x, pointer.anchoredPosition.y);
    }

    void Update()
    {
        if (isClicked)
        {
            /*
             * CanvasのRectTransform内にあるマウスの位置をRectTransformのローカルポジションに変換する
             * canvas.worldCameraはカメラ
             * 出力先はMousePos
             */
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, canvas.worldCamera, out MousePos);
            radian = (float)Math.Atan2(MousePos.y - init.y, MousePos.x - init.x); //x-z平面のtanの値計算
            // RectTransformの座標を更新
            if (Vector2.Distance(init, MousePos) <= leverRange)
                pointer.anchoredPosition = new Vector2(MousePos.x, MousePos.y);
            else if (Vector2.Distance(init, MousePos) > leverRange) // 可動域よりも外側にカーソルがある場合
            {
                pointer.anchoredPosition = new Vector2(init.x + (leverRange * (float)Math.Cos(radian)), init.y + (leverRange * (float)Math.Sin(radian))); // 基準値にオフセットを足して座標とする
            }

            //Flagの処理
            if (Vector2.Distance(init, MousePos) >= leverRange / 2.0f)
            {
                Debug.Log(radian / Math.PI + "pi");
                for (int i = 0; i < 7; i++)
                {
                    if (radian > (-7 + 2 * i) * Math.PI / 8 && radian <= (-5 + 2 * i) * Math.PI / 8) // レバーの方向を検知，Flagを場合分け．radianは弧度法による値:-Pi to Pi
                    {
                        switch (i)
                        {
                            case 0:
                                InitializeFlag();
                                leftFlag = true;
                                forwardFlag = true;
                                break;
                            case 1:
                                InitializeFlag();
                                forwardFlag = true;
                                break;
                            case 2:
                                InitializeFlag();
                                rightFlag = true;
                                forwardFlag = true;
                                break;
                            case 3:
                                InitializeFlag();
                                rightFlag = true;
                                break;
                            case 4:
                                InitializeFlag();
                                rightFlag = true;
                                backFlag = true;
                                break;
                            case 5:
                                InitializeFlag();
                                backFlag = true;
                                break;
                            case 6:
                                InitializeFlag();
                                leftFlag = true;
                                backFlag = true;
                                break;
                        }
                    }
                }
                if (radian > 7 * Math.PI / 8 && radian <= Math.PI || radian >= -Math.PI && radian <= -7 * Math.PI / 8) // 左端は-と+が交じるため特別な処理
                {
                    InitializeFlag();
                    leftFlag = true;
                }
            }
            else InitializeFlag(); // 距離によってレバーが動作しないようにする
        }
    }

    public void ButtonDown()
    {
        isClicked = true;
    }

    public void ButtonUp()
    {
        isClicked = false;
        pointer.anchoredPosition = init;
        InitializeFlag();
    }

    void InitializeFlag()
    {
        rightFlag = false;
        leftFlag = false;
        backFlag = false;
        forwardFlag = false;
    }
}
