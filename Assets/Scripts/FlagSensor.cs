using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FlagSensor : MonoBehaviour
{
    [SerializeField] HingeJoint h;
    bool isCollis;
    [SerializeField] bool timeValidation = true;

    async void Unlock()
    {
        if (timeValidation)
        {
            await Task.Delay(2000);
            if (!isCollis) return;
        }
        if (h.useLimits) h.useLimits = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Flag"))
        {
            isCollis = true;
            Unlock();
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Flag"))
            isCollis = false;
    }
}
