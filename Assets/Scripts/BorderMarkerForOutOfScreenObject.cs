using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script places an UI-Object on the screen borders to mark the direction where an off screen object is placed.
/// </summary>
public class BorderMarkerForOutOfScreenObject : MonoBehaviour
{
    [SerializeField] Transform followedTarget;

    [SerializeField] Camera usedCamera;

    /// <summary>
    /// the rect transform that is positioned by the script
    /// </summary>
    [SerializeField] RectTransform markerAnchor;

    /// <summary>
    /// This should be a child of this class' gameobject.
    /// </summary>
    [SerializeField] GameObject markerIconGameObject;

    /// <summary>
    /// the canvas this marker is in
    /// </summary>
    private Canvas markerCanvas;

    [SerializeField] Vector2 borderHorizontal = new Vector4(15f, 15f);
    [SerializeField] Vector2 borderVertical = new Vector4(15f, 15f);

    private void Awake()
    {
        markerCanvas = markerAnchor.GetComponentInParent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 targetRelativePosition = usedCamera.WorldToViewportPoint(followedTarget.position);

        markerIconGameObject.SetActive(targetRelativePosition.x < 0 || targetRelativePosition.x > 1 || targetRelativePosition.y < 0 || targetRelativePosition.y > 1);

        if (!markerIconGameObject.activeSelf) return;

        Vector3 screenPosition = usedCamera.ViewportToScreenPoint(targetRelativePosition);

        float canvasScale = markerCanvas.transform.localScale.y;

        screenPosition.x = Mathf.Clamp(screenPosition.x, borderHorizontal.x * canvasScale, markerCanvas.pixelRect.width - borderHorizontal.y * canvasScale);
        screenPosition.y = Mathf.Clamp(screenPosition.y, borderVertical.x * canvasScale, markerCanvas.pixelRect.height - borderVertical.y * canvasScale);
        screenPosition.z = 0;

        screenPosition.x /= canvasScale;
        screenPosition.y /= canvasScale;
        
        markerAnchor.anchoredPosition = screenPosition;
    }
}
