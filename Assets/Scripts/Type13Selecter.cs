using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type13Selecter : MonoBehaviour
{
    [SerializeField] Type13Manager[] manager = new Type13Manager[2];
    [SerializeField] Animator[] animator = new Animator[2];
    BGMPlayer bp;
    [SerializeField] float audioPitch = 1.0f;
    [SerializeField] int lightColor = 1; //1:Red, 2:Green, 3:Blue, 4:Yellow, 5:Sky, 6:Purple, 7:Pink(255,128,255), 8:Orange(255,128,0), 9:Forest(128,255,0)
    // Start is called before the first frame update
    void Start()
    {
        bp = transform.Find("BGM").GetComponent<BGMPlayer>();
        manager[0] = transform.Find("1P").GetComponent<Type13Manager>();
        manager[1] = transform.Find("2P").GetComponent<Type13Manager>();
        bp.SetAudioPitch(audioPitch);

        for (int i = 0; i < 2; i++)
        {
            animator[i].SetInteger("ColorNumber", lightColor);
            manager[0].animator[i] = animator[i];
            manager[1].animator[i] = animator[i];
            manager[i].sp.SetAudioPitch(audioPitch);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (manager[0].boothStatus == 0 && manager[1].boothStatus == 0)
        {
            bp.Stop(1);
            bp.Play(0);
        }

        else if (manager[0].boothStatus > 0 || manager[1].boothStatus > 0)
        {
            bp.Stop(0);
            bp.Play(1);
        }
    }
}
