using System.Collections;
using UnityEngine;

public class FlagSensor : MonoBehaviour
{
    [SerializeField] HingeJoint h;
    bool isCollis;
    [SerializeField] bool timeValidation = true;

    public void Unlock()
    {
        StartCoroutine(InternalUnlock());
    }

    IEnumerator InternalUnlock()
    {
        if (timeValidation)
        {
            yield return new WaitForSeconds(2);
            if (isCollis && h.useLimits) h.useLimits = false;
        }
        else
        {
            if (h.useLimits) h.useLimits = false;
        }
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
