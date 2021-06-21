using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type4Selecter : MonoBehaviour
{
    [SerializeField] Type4Manager[] manager = new Type4Manager[2];
    BGMPlayer _BGMPlayer;
    SEPlayer[] _SEPlayer = new SEPlayer[2];

    void Start()
    {
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
        _SEPlayer[0] = this.transform.Find("1P").Find("SE").GetComponent<SEPlayer>();
        _SEPlayer[1] = this.transform.Find("2P").Find("SE").GetComponent<SEPlayer>();
    }

    void Update()
    {
        if (manager[0].craneStatus == 0 && manager[1].craneStatus == 0)
        {
            if (!_BGMPlayer._AudioSource[0].isPlaying)
            {
                _BGMPlayer.StopBGM(1);
                _BGMPlayer.PlayBGM(0);
            }
        }
        else if (manager[0].craneStatus == 15 || manager[1].craneStatus == 15 || _SEPlayer[0]._AudioSource[6].isPlaying || _SEPlayer[1]._AudioSource[6].isPlaying) _BGMPlayer.StopBGM(1);
        else if ((manager[0].craneStatus > 0 || manager[1].craneStatus > 0) && (manager[0].craneStatus < 15 || manager[1].craneStatus < 15))
        {
            if (!_BGMPlayer._AudioSource[1].isPlaying && (!_SEPlayer[0]._AudioSource[6].isPlaying || !_SEPlayer[1]._AudioSource[6].isPlaying))
            {
                _BGMPlayer.StopBGM(0);
                _BGMPlayer.PlayBGM(1);
            }
        }
    }

    public Type4Manager GetManager(int num)
    {
        return manager[num - 1];
    }
}
