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
        // videoManager[0].Play(temp);
        // videoManager[1].Play(temp);
        // StartCoroutine("RandomVideoPlay2");
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
        else if (manager[0].GetStatus() == 15 || manager[1].GetStatus() == 15) bp.Stop(1);
    }

    // IEnumerator RandomVideoPlay2()
    // {
    //     int randomValue;
    //     float playTime;

    //     while (true)
    //     {
    //         randomValue = Random.Range(0, videoManager[0].videoClips.Length);
    //         playTime = Random.Range(3, 8);
    //         if (manager[0].GetStatus() == 0 && videoManager[0].randomMode) videoManager[0].Play(randomValue);
    //         if (manager[1].GetStatus() == 0 && videoManager[1].randomMode) videoManager[1].Play(randomValue);
    //         yield return new WaitForSeconds(playTime);
    //     }
    // }

    // async void RandomVideoPlay()
    // {
    //     int randomValue;
    //     float playTime;

    //     while (true)
    //     {
    //         randomValue = Random.Range(0, videoManager[0].videoClips.Length);
    //         playTime = Random.Range(3, 8);
    //         if (manager[0].GetStatus() == 0 && videoManager[0].randomMode) videoManager[0].Play(randomValue);
    //         if (manager[1].GetStatus() == 0 && videoManager[1].randomMode) videoManager[1].Play(randomValue);
    //         await Task.Delay((int)(playTime * 1000));
    //     }
    // }

    public Type12Manager GetManager(int num)
    {
        return manager[num - 1];
    }
}
