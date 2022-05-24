using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarSlider;
    [SerializeField] private Image healthBarBackground;

    private PlayerPawn myPawn;

    [Space] float heightOffset = 8.5f;

    bool isWounded;
    private Camera mainCam;
    private RectTransform myTransform;

    private RectTransform targetCanvasTransform;

    private void Awake()
    {
        mainCam = Camera.main;
        // myTransform = transform as RectTransform; // null if transform isn't RectTransform.
        myTransform = (RectTransform)transform;     // throwns error if transform isn't RectTransform.
    }

    /// <summary>
    /// Updates the position of the healthbar if component is enabled.
    /// </summary>
    private void Update()
    {
        UpdatePosition();
    }

    /// <summary>
    /// Called to set up values when a new Healthbar is created.
    /// </summary>
    public void Initialised(PlayerPawn pawn, Canvas targetCanvas)
    {
        myPawn = pawn;
        this.targetCanvasTransform = (RectTransform)targetCanvas.transform;

        UpdateHealthBar();
    }

    /// <summary>
    /// Changes how far the health bar is filled and if it's visible at all.
    /// </summary>
    public void UpdateHealthBar()
    {
        float hp01 = (float)myPawn.HP / (float)myPawn.MaxHealth;

        healthBarSlider.fillAmount = hp01;

        isWounded = hp01 < 1f;

        SetVisible(isWounded);
        enabled = isWounded;
    }

    /// <summary>
    /// Updates the position of the healthbar according to the pawns position.
    /// </summary>
    private void UpdatePosition()
    {
        Vector3 usedPosition = myPawn.WorldPosition;
        usedPosition.y += heightOffset;

        Vector2 viewportPosition = mainCam.WorldToViewportPoint(usedPosition);

        Vector2 screenPosition = new Vector2(
       (viewportPosition.x - 0.5f) * targetCanvasTransform.sizeDelta.x, 
       (viewportPosition.y - 0.5f) * targetCanvasTransform.sizeDelta.y);

        myTransform.anchoredPosition = screenPosition;
    }

    /// <summary>
    /// Changes if the Healthbar is shown.
    /// </summary>
    private void SetVisible(bool isVisible)
    {
        if (isVisible)
            UpdatePosition();

        healthBarBackground.enabled = isVisible;
        healthBarSlider.enabled = isVisible;

    }
}
