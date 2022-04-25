using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverHexCheck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool onHex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onHex = true;
     //   Debug.Log("overHex" + overHex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onHex=false;
     //   Debug.Log("overHex" + overHex);
    }

}
