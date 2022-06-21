using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type11Selecter : MonoBehaviour
{
    [SerializeField] Type11Manager[] manager = new Type11Manager[2];
    BGMPlayer bp;
    [SerializeField] float audioPitch = 1.0f;
    int nextPlay = 1; // 1:初期音源，2：ループ音源，3：Bonus音源
    // Start is called before the first frame update
    void Start()
    {
        bp = transform.Find("BGM").GetComponent<BGMPlayer>();
        bp.SetAudioPitch(audioPitch);
        manager[0].sp.SetAudioPitch(audioPitch);
        manager[1].sp.SetAudioPitch(audioPitch);
    }

    // Update is called once per frame
    void Update()
    {
        if (manager[0].GetStatus() == 0 && manager[1].GetStatus() == 0)
        {
            bp.Stop(1);
            bp.Stop(2);
            bp.Stop(3);
            bp.Play(0);
            nextPlay = 1;
        }

        else if (manager[0].GetStatus() > 0 || manager[1].GetStatus() > 0)
        {
            bp.Stop(0);
            // ボーナスゲームチェック
            if (manager[0].isBonus || manager[1].isBonus)
            {
                bp.Stop(1);
                bp.Stop(2);
                bp.Play(3);
                nextPlay = 1;
            }
            else
            {
                bp.Stop(3);
                if (!bp.audioSource[1].isPlaying)
                {
                    if (nextPlay == 1)
                    {
                        bp.Play(1);
                        nextPlay = 2;
                    }
                    else
                    {
                        bp.Play(2);
                    }
                }
            }
        }
    }
}
