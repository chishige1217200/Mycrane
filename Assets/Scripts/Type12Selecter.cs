using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type12Selecter : MonoBehaviour
{
    [SerializeField] Type12Manager[] manager = new Type12Manager[2];
    [SerializeField] Type12LightManager[] lightManager = new Type12LightManager[2];
    [SerializeField] int operationType = 1;
    BGMPlayer bp;
    SEPlayer[] sp = new SEPlayer[2];

    void Start()
    {
        manager[0].operationType = operationType;
        manager[1].operationType = operationType;
        // int temp = Random.Range(0, videoManager[0].videoClips.Length);
        bp = transform.Find("BGM").GetComponent<BGMPlayer>();
        sp[0] = transform.Find("1P").Find("SE").GetComponent<SEPlayer>();
        sp[1] = transform.Find("2P").Find("SE").GetComponent<SEPlayer>();
        lightManager[0] = transform.Find("1P").Find("CraneUnit").Find("ArmUnit").Find("Main").Find("LightGroup").GetComponent<Type12LightManager>();
        lightManager[1] = transform.Find("2P").Find("CraneUnit").Find("ArmUnit").Find("Main").Find("LightGroup").GetComponent<Type12LightManager>();
    }

    void Update()
    {
        if (!bp.audioSource[0].isPlaying && !sp[0].audioSource[7].isPlaying && !sp[0].audioSource[8].isPlaying && !sp[1].audioSource[7].isPlaying && !sp[1].audioSource[8].isPlaying && (manager[0].GetStatus() == 0 && manager[1].GetStatus() == 0))
        {
            bp.Stop(1);
            bp.Play(0);
        }
        else if (!sp[0].audioSource[7].isPlaying && !sp[0].audioSource[8].isPlaying && !sp[1].audioSource[7].isPlaying && !sp[1].audioSource[8].isPlaying && (manager[0].GetStatus() >= 1 || manager[1].GetStatus() >= 1))
        {
            bp.Stop(0);

            if ((manager[0].GetStatus() < 8 && manager[1].GetStatus() < 8))
            {
                if (!bp.audioSource[1].isPlaying) bp.Play(1);
            }
            else bp.Stop(1);
        }
        else if (manager[0].GetStatus() == 19 || manager[1].GetStatus() == 19) bp.Stop(1);
    }

    public void LightReset()
    {
        if (manager[0].GetStatus() == 0)
        {
            lightManager[0].Pattern(0);
        }

        if (manager[1].GetStatus() == 0)
        {
            lightManager[1].Pattern(0);
        }
    }
}
