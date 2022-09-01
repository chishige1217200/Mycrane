using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type13Selecter : MonoBehaviour
{
    [SerializeField] Type13Manager[] manager = new Type13Manager[2];
    [SerializeField] Animator[] animator = new Animator[3];
    BGMPlayer bp;
    [SerializeField] float audioPitch = 1.0f;
    [SerializeField] int lightColor = 1; //1:Red, 2:Green, 3:Blue, 4:Yellow, 5:Sky, 6:Purple, 7:Pink(255,128,255), 8:Orange(255,128,0), 9:Forest(128,255,0)
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
