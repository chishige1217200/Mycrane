using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type4Selecter : MonoBehaviour
{
    [SerializeField] Type4Manager[] manager = new Type4Manager[2];
    BGMPlayer _BGMPlayer;

    void Start()
    {
        _BGMPlayer = this.transform.Find("BGM").GetComponent<BGMPlayer>();
    }

    async void Update()
    {
        if (manager[0].craneStatus == 0 && manager[1].craneStatus == 0)
        {
            if (!_BGMPlayer._AudioSource[0].isPlaying)
            {
                _BGMPlayer.StopBGM(1);
                _BGMPlayer.PlayBGM(0);
            }
        }
        else if (manager[0].craneStatus == 15 || manager[1].craneStatus == 15) _BGMPlayer.StopBGM(1);
        else if ((manager[0].craneStatus > 0 || manager[1].craneStatus > 0) && (manager[0].craneStatus < 15 || manager[1].craneStatus < 15))
        {
            await Task.Delay(500);
            if (!_BGMPlayer._AudioSource[1].isPlaying)
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
