﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    public Canvas canvas; // Canvasの変数
    public RectTransform canvasRect; // キャンバス内のレクトトランスフォーム
    public Vector2 mousePos; // マウスの位置の最終的な格納先
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
             * 出力先はmousePos
             */
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, canvas.worldCamera, out mousePos);
            radian = (float)Math.Atan2(mousePos.y - init.y, mousePos.x - init.x); //x-z平面のtanの値計算
            // RectTransformの座標を更新
            if (Vector2.Distance(init, mousePos) <= leverRange)
                pointer.anchoredPosition = new Vector2(mousePos.x, mousePos.y);
            else if (Vector2.Distance(init, mousePos) > leverRange) // 可動域よりも外側にカーソルがある場合
            {
                pointer.anchoredPosition = new Vector2(init.x + (leverRange * (float)Math.Cos(radian)), init.y + (leverRange * (float)Math.Sin(radian))); // 基準値にオフセットを足して座標とする
            }

            //Flagの処理
            if (Vector2.Distance(init, mousePos) >= leverRange / 2.0f)
            {
                //Debug.Log(radian / Math.PI + "pi");
                Initialize();                       // 事前に方向を初期化
                for (int i = 0; i < 7; i++)
                {
                    if (radian > (-7 + 2 * i) * Math.PI / 8 && radian <= (-5 + 2 * i) * Math.PI / 8) // レバーの方向を検知，Flagを場合分け．radianは弧度法による値:-Pi to Pi
                    {
                        switch (i)
                        {
                            case 0:
                                leftFlag = true;
                                forwardFlag = true;
                                break;
                            case 1:
                                forwardFlag = true;
                                break;
                            case 2:
                                rightFlag = true;
                                forwardFlag = true;
                                break;
                            case 3:
                                rightFlag = true;
                                break;
                            case 4:
                                rightFlag = true;
                                backFlag = true;
                                break;
                            case 5:
                                backFlag = true;
                                break;
                            case 6:
                                leftFlag = true;
                                backFlag = true;
                                break;
                        }
                    }
                }
                if (radian > 7 * Math.PI / 8 && radian <= Math.PI || radian >= -Math.PI && radian <= -7 * Math.PI / 8) // 左端は-と+が交じるため特別な処理
                    leftFlag = true;
            }
            else Initialize(); // 距離によってレバーが動作しないようにする
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
        Initialize();
    }

    void Initialize()
    {
        rightFlag = false;
        leftFlag = false;
        backFlag = false;
        forwardFlag = false;
    }
}
