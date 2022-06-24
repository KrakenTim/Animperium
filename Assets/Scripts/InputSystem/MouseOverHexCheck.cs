using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Simple class telling if the cursor is currently over a hex grid field or not.
/// </summary>
public class MouseOverHexCheck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool IsOnHex { get; private set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsOnHex = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsOnHex = false;
    }
}
