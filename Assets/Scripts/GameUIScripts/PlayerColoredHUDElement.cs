using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerColoredHUDElement : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)] float playerTint = 0.25f;

    Color originalColor;
    Image myImage;

   [SerializeField] bool useActiveInsteadOfLocal = false;

    private void Awake()
    {
        myImage = GetComponent<Image>();
        originalColor = myImage.color;
    }

    private void OnEnable()
    {
        if (useActiveInsteadOfLocal)
        {
            GameManager.TurnStarted += UpdateColor;
            UpdateColor(GameManager.ActivePlayerID);
        }
        else
        {
            GameManager.LocalPlayerChanged += UpdateWithLocalColor;
            UpdateWithLocalColor();
        }
    }

    private void OnDisable()
    {
        if (useActiveInsteadOfLocal)
            GameManager.TurnStarted -= UpdateColor;
        else
            GameManager.LocalPlayerChanged -= UpdateWithLocalColor;
    }

    private void UpdateColor(int playerID)
    {
        myImage.color = Color.Lerp(originalColor, GameManager.GetPlayerColor(playerID), playerTint);
    }

    private void UpdateWithLocalColor()
    {
        UpdateColor(GameManager.LocalPlayerID);
    }
}
