using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Color currentColor;

    private void Start()
    {
        currentColor = GetComponent<MeshRenderer>().material.color;
    }

   void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
        GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
        GetComponent<MeshRenderer>().material.color = currentColor;
    }
}
