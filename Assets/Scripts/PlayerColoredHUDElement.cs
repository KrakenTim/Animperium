using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerColoredHUDElement : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)] float playerTint = 0.25f;

    Color originalColor;
    Image myImage;

    private void Awake()
    {
        myImage = GetComponent<Image>();
        originalColor = myImage.color;
    }

    private void OnEnable()
    {
        GameManager.LocalPlayerChanged += UpdateColor;
        UpdateColor();
    }

    private void OnDisable()
    {
        GameManager.LocalPlayerChanged -= UpdateColor;
    }

    private void UpdateColor()
    {
        myImage.color = Color.Lerp(originalColor, GameManager.GetPlayerColor(GameManager.LocalPlayerID), playerTint);
    }
}
