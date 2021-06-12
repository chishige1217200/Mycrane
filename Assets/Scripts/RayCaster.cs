using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    Ray ray; // Ray情報
    RaycastHit hit; // 衝突した物体についての情報
    MachineHost host; // 現在プレイ中の筐体情報
    bool isFirst = true; // 初回時のみtrue

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) Cast(); // 右クリック時
    }

    void Cast() // Rayを飛ばす
    {
        ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, 3);
        if (hit.collider.gameObject.name == "CP") // 操作パネルにRayがあたった場合（要コライダー）
        {
            if (!isFirst) host.playable = false; // 古い方の筐体は再度ロック
            host = hit.collider.gameObject.GetComponent<MachineHost>();
            host.playable = true;
            isFirst = false;
            Debug.Log("Activate Successfully.");
        }
        else
        {
            if (!isFirst) host.playable = false; // 古い方の筐体は再度ロック
        }
    }
}
