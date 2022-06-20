using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type11Selecter : MonoBehaviour
{
    [SerializeField] Type11Manager[] manager = new Type11Manager[2];
    BGMPlayer bp;
    [SerializeField] float audioPitch = 1.0f;
    int nextPlay = 6;
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
            bp.Play(0);
    }
}
