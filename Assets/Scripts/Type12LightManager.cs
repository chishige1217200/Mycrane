using UnityEngine;
using UnityEngine.Video;
using System.Threading.Tasks;

public class Type12LightManager : MonoBehaviour
{
    Animator light;

    void Start()
    {
        light = GetComponent<Animator>();
    }

    public void Pattern(int num)
    {
        light.SetInteger("LightNumber", num);
    }

    public void Reset()
    {
        light.SetTrigger("Reset");
    }
}
