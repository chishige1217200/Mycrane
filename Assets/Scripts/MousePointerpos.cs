using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MousePointerpos : MonoBehaviour
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
        Debug.Log(init);
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
            pointer.anchoredPosition = new Vector2(MousePos.x, MousePos.y);
        }
    }

    public void ClickDown()
    {
        Debug.Log("Pointer Down");
        isClicked = true;
    }

    public void ClickUp()
    {
        Debug.Log("Pointer Up");
        isClicked = false;
        Debug.Log(init);
        pointer.anchoredPosition = init;
    }

}
