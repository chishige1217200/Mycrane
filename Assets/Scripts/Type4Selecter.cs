using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type4Selecter : MonoBehaviour
{
    [SerializeField] Type4Manager[] manager = new Type4Manager[2];
    [SerializeField] Type4VideoPlay[] videoPlay = new Type4VideoPlay[2];
    BGMPlayer _BGMPlayer;
    SEPlayer[] _SEPlayer = new SEPlayer[2];

    void Start()
    {
        int temp = Random.Range(0, videoPlay[0].videoClips.Length);
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer[0] = this.transform.Find("1P").Find("SE").GetComponent<SEPlayer>();
        _SEPlayer[1] = this.transform.Find("2P").Find("SE").GetComponent<SEPlayer>();
        videoPlay[0].PlayVideo(temp);
        videoPlay[1].PlayVideo(temp);
        RandomVideoPlay();
    }

    void Update()
    {
        if (manager[0].craneStatus == 0 && manager[1].craneStatus == 0)
        {
            if (!_BGMPlayer._AudioSource[0].isPlaying)
            {
                _BGMPlayer.Stop(1);
                _BGMPlayer.Play(0);
            }
        }
        else if (manager[0].craneStatus == 15 || manager[1].craneStatus == 15 || _SEPlayer[0]._AudioSource[6].isPlaying || _SEPlayer[1]._AudioSource[6].isPlaying) _BGMPlayer.Stop(1);
        else if ((manager[0].craneStatus > 0 || manager[1].craneStatus > 0) && (manager[0].craneStatus < 15 || manager[1].craneStatus < 15))
        {
            if (!_BGMPlayer._AudioSource[1].isPlaying && (!_SEPlayer[0]._AudioSource[6].isPlaying || !_SEPlayer[1]._AudioSource[6].isPlaying))
            {
                _BGMPlayer.Stop(0);
                _BGMPlayer.Play(1);
            }
        }
    }

    async void RandomVideoPlay()
    {
        int randomValue;
        float playTime;

        while (true)
        {
            randomValue = Random.Range(0, videoPlay[0].videoClips.Length);
            playTime = Random.Range(3, 8);
            if (manager[0].craneStatus == 0 && videoPlay[0].randomMode) videoPlay[0].PlayVideo(randomValue);
            if (manager[1].craneStatus == 0 && videoPlay[1].randomMode) videoPlay[1].PlayVideo(randomValue);
            await Task.Delay((int)(playTime * 1000));
        }

    }

    public Type4Manager GetManager(int num)
    {
        return manager[num - 1];
    }
}
