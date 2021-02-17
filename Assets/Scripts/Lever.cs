using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    //Canvasの変数
    public Canvas canvas;
    //キャンバス内のレクトトランスフォーム
    public RectTransform canvasRect;
    //マウスの位置の最終的な格納先
    public Vector2 MousePos;
    //自身のゲームオブジェクトのRectTransform
    public RectTransform pointer;
    bool isClicked = false;
    Vector2 init; // レバーの基準初期座標
    Type1Manager _Type1Manager;
    Type2Manager _Type2Manager;
    Type3Manager _Type3Manager;
    int craneType = -1;
    float leverRange = 30f; // レバーの可動域
    float radian = 0f; // 角度計算用

    // Start is called before the first frame update
    void Start()
    {
        //マウスポインター非表示
        //Cursor.visible = false;
        //HierarchyにあるCanvasオブジェクトを探してcanvasに入れいる
        //canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        //canvas内にあるRectTransformをcanvasRectに入れる
        canvasRect = canvas.GetComponent<RectTransform>();
        //自身のゲームオブジェクトのRectTransformをpointerに入れる
        pointer = this.GetComponent<RectTransform>();
        init = new Vector2(pointer.anchoredPosition.x, pointer.anchoredPosition.y);
        //Debug.Log(init);
    }

    // Update is called once per frame
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
            //RectTransformの座標を更新
            if (Vector2.Distance(init, MousePos) <= leverRange)
                pointer.anchoredPosition = new Vector2(MousePos.x, MousePos.y);
            else if (Vector2.Distance(init, MousePos) > leverRange)
            {
                radian = (float)Math.Atan2(MousePos.y - init.y, MousePos.x - init.x); //x-z平面のtanの値計算
                pointer.anchoredPosition = new Vector2(init.x + (leverRange * (float)Math.Cos(radian)), init.y + (leverRange * (float)Math.Sin(radian)));
            }
        }
    }

    public void GetManager(int num)
    {
        craneType = num;
        if (craneType == 1)
            _Type1Manager = transform.root.gameObject.GetComponent<Type1Manager>();
        if (craneType == 2)
            _Type2Manager = transform.root.gameObject.GetComponent<Type2Manager>();
        if (craneType == 3)
            _Type3Manager = transform.root.gameObject.GetComponent<Type3Manager>();
    }

    public void ClickDown()
    {
        //Debug.Log("Pointer Down");
        isClicked = true;
    }

    public void ClickUp()
    {
        //Debug.Log("Pointer Up");
        isClicked = false;
        //Debug.Log(init);
        pointer.anchoredPosition = init;
    }

}
