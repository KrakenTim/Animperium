using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorableImage : MonoBehaviour
{
    [SerializeField] Image coloredIcon;
    [SerializeField] Image baseIcon;
    [SerializeField] TMPro.TMP_Text text;
    [Space]
    [SerializeField, Range(0.125f, 1f)] float onlyColoredIntensity = 0.5f;

    /// <summary>
    /// less efficient, if possible use PlayerValues-variant instead
    /// </summary>
    public void SetPlayer(int playerID)
    {
        // TODO (22-05-09) less ugly
        Set(GameManager.GetPlayerIcon(playerID), GameManager.GetPlayerColor(playerID));
    }

    public void SetPlayer(PlayerValues playerData)
    {
        Set(playerData.PlayerIcon, playerData.playerColor);
    }

    public void SetPawn(PlayerPawn pawn)
    {
        SetPawn(pawn.PawnIcon, pawn.PlayerID);
    }

    public void SetPawn(PlayerPawnData pawnData, int playerID)
    {
        SetPawn(pawnData.PawnIcon, playerID);
    }

    public void SetPawn(ColorableIconData iconData, int playerID)
    {
        Set(iconData, GameManager.GetPlayerColor(playerID));
    }

    public void Set(ColorableIconData iconData, Color playerColor)
    {
        if (iconData.colored)
        {
            coloredIcon.sprite = iconData.colored;
            // colors image, if there's no base it's possibly less intensive
            coloredIcon.color = iconData.baseIcon != null ? playerColor : Color.Lerp(Color.white, playerColor, onlyColoredIntensity);
            coloredIcon.enabled = true;
        }
        else
            coloredIcon.enabled = false;

        if (iconData.baseIcon)
        {
            baseIcon.sprite = iconData.baseIcon;
            baseIcon.enabled = true;
        }
        else
            baseIcon.enabled = false;

        if (!string.IsNullOrEmpty(iconData.text))
        {
            text.text = iconData.text;
            text.enabled = true;
        }
        else
            text.enabled = false;
    }

    public void SetVisible(bool isEnabled)
    {
        coloredIcon.enabled = isEnabled;
        baseIcon.enabled = isEnabled;
        text.enabled = isEnabled;
    }
}
