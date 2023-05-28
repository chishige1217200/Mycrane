using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type5SelecterV3 : CelebrationSync
{
    public Type5SelecterConfig config;
    [SerializeField] Type5ManagerV3[] manager = new Type5ManagerV3[2];
    [SerializeField] Animator[] animator = new Animator[3];
    [SerializeField] int lightColor = 1; //1:Red, 2:Green, 3:Blue, 4:Yellow, 5:Sky, 6:Purple, 7:Pink(255,128,255), 8:Orange(255,128,0), 9:Forest(128,255,0)
    private BGMPlayer bp;
    private int nextPlay = 6;

    void Start()
    {
        bp = transform.Find("BGM").GetComponent<BGMPlayer>();

        for (int i = 0; i < 3; i++)
        {
            animator[i].SetInteger("ColorNumber", lightColor);
            manager[0].animator[i] = animator[i];
            manager[1].animator[i] = animator[i];
        }

        if(config != null)
        {
            Setup(config);
        }
    }

    public void Setup(Type5SelecterConfig config)
    {
        this.config = config;
        bp.SetAudioPitch(config.audioPitch);
        manager[0].soundType = config.soundType;
        manager[1].soundType = config.soundType;
        manager[0].sp.SetAudioPitch(config.audioPitch);
        manager[1].sp.SetAudioPitch(config.audioPitch);
    }

    void Update()
    {
        if (manager[0].craneStatus == 0 && manager[1].craneStatus == 0)
        {
            switch (config.soundType)
            {
                case 0:
                    bp.Stop(1);
                    bp.Play(0);
                    break;
                case 1:
                    bp.Stop(3);
                    bp.Play(2);
                    break;
                case 2:
                    bp.Stop(5);
                    bp.Play(4);
                    break;
                case 3:
                    bp.Stop(8);
                    if (!bp.audioSources[6].isPlaying && !bp.audioSources[7].isPlaying)
                    {
                        bp.Play(nextPlay);
                        if (nextPlay == 6) nextPlay = 7;
                        else if (nextPlay == 7) nextPlay = 6;
                    }
                    break;
            }
        }
        else if (manager[0].craneStatus > 0 || manager[1].craneStatus > 0)
        {
            switch (config.soundType)
            {
                case 0:
                    bp.Stop(0);
                    bp.Play(1);
                    break;
                case 1:
                    bp.Stop(2);
                    bp.Play(3);
                    break;
                case 2:
                    bp.Stop(4);
                    bp.Play(5);
                    break;
                case 3:
                    bp.Stop(6);
                    bp.Stop(7);
                    bp.Play(8);
                    break;
            }
        }
    }

    public override void SetupNetWork(Type5NetworkV3 net) // 獲得連動用
    {
        manager[0].net = net;
        manager[1].net = net;
    }

    public override void Celebrate() // 獲得連動用
    {
        for (int i = 0; i < 3; i++) animator[i].SetTrigger("GetPrize");
    }
}
