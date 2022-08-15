using UnityEngine;
using UnityEngine.Video;
using System.Threading.Tasks;

public class Type12LightManager : MonoBehaviour
{
    Animator ani;

    void Start()
    {
        ani = GetComponent<Animator>();
    }

    public void Pattern(int num)
    {
        ani.SetInteger("LightNumber", num);
        Reset();
    }

    void Reset()
    {
        ani.SetTrigger("Reset");
    }
}
