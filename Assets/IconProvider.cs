using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ColorableIcon
{
    public Sprite colored;
    public Sprite baseIcon;
}
public class IconProvider : MonoBehaviour
{
    private static IconProvider instance;

    [SerializeField] ColorableIcon fallbackUnit;
    [SerializeField] ColorableIcon fallbackBuilding;
    [SerializeField] ColorableIcon fallbackPlayer;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public static ColorableIcon GetUnitIcon()
    {
        return instance.fallbackUnit;
    }

    public static ColorableIcon GetBuildingIcon()
    {
        return instance.fallbackBuilding;
    }

    public static ColorableIcon GetPlayerIcon()
    {
        return instance.fallbackPlayer;
    }

    public static void SetupPawn(ref Image coloredImage, ref Image baseImage, int playerID, PlayerPawnData pawnData)
    {
        Color playerColor = GameManager.GetPlayerColor(playerID);
        ColorableIcon pawnIcon = pawnData.pawnIcon;

        if (pawnIcon.colored == null && pawnIcon.baseIcon == null)
        {
            if (pawnData.IsBuilding)
                pawnIcon = instance.fallbackBuilding;
            else
                pawnIcon = instance.fallbackUnit;
        }

        instance.ColorIcon(ref coloredImage, ref baseImage, playerColor, pawnIcon);
    }

    public static void SetupPlayer(ref Image coloredImage, ref Image baseImage, int playerID)
    {
        Color playerColor = GameManager.GetPlayerColor(playerID);
        ColorableIcon playerIcon = GameManager.GetPlayerIcon(playerID);

        if (playerIcon.colored == null && playerIcon.baseIcon == null)
            playerIcon = instance.fallbackPlayer;

        instance.ColorIcon(ref coloredImage, ref baseImage, playerColor, playerIcon);
    }

    private void ColorIcon(ref Image coloredImage, ref Image baseImage, Color usedColor, ColorableIcon icon)
    {
        if (coloredImage)
        {
            coloredImage.sprite = icon.colored;
            // colors image, if there's no base is's less intensive
            coloredImage.color = icon.baseIcon != null ? usedColor : Color.Lerp(usedColor, Color.white, 0.5f);
        }

        if (baseImage)
        {
            baseImage.sprite = icon.baseIcon;

            // if there's no image and there's no base icon, use colored for base.
            if (coloredImage == null && icon.baseIcon == null)
                baseImage.sprite = icon.colored;
        }
    }
}
