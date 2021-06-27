using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class sortingLayer : MonoBehaviour
{
    public string sortingLayerName;
    public int sortingOrder;

    // Use this for initialization
    void Start()
    {
        Renderer curRenderer = gameObject.GetComponent<MeshRenderer>();
        if (curRenderer == null)
            return;
        curRenderer.sortingLayerName = sortingLayerName;
        curRenderer.sortingOrder = sortingOrder;
    }
}