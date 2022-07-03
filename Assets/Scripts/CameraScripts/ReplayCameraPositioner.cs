using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayCameraPositioner : MonoBehaviour
{
    public static event System.Action FinishedShift;

    [SerializeField] Transform shiftedTransform;

    public bool IsShiftNeeded(InputMessage order, out Vector3 position)
    {
        // no camera means nothing to shift
        if (shiftedTransform == null)
        {
            position = Vector3.zero;
            return false;
        }

        position = shiftedTransform.position;

        HexCell start = HexGridManager.Current.GetHexCell(order.startCoordinates,order.startLayer);
        HexCell end = HexGridManager.Current.GetHexCell(order.targetCoordinates,order.targetLayer);

        position = (start.transform.position + end.transform.position) / 2f;

        return !IsInInnerArea(position);
    }

    public void SetPosition(Vector3 position)
    {
        shiftedTransform.position = position;
    }

    private bool IsInInnerArea(Vector3 position)
    {
        Vector3 viewPosition = Camera.main.WorldToViewportPoint(position);

        return viewPosition.x > 0.33333f && viewPosition.x < 0.66666f && viewPosition.y > 0.33333f && viewPosition.y < 0.66666f;
    }
}
