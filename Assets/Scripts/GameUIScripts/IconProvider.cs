using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ColorableIconData
{
    [HideInInspector] public string text;
    public Sprite colored;
    public Sprite baseIcon;
}

public class IconProvider : MonoBehaviour
{
    private static IconProvider instance;

    [SerializeField] ColorableIconData fallbackUnit;
    [SerializeField] ColorableIconData fallbackBuilding;
    [SerializeField] ColorableIconData fallbackPlayer;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public static ColorableIconData GetCheckedPawn(ColorableIconData pawnIcon, ePlayerPawnType type)
    {
        if (pawnIcon.colored == null && pawnIcon.baseIcon == null)
        {
            pawnIcon = type.IsBuilding() ? instance.fallbackBuilding : instance.fallbackUnit;

            pawnIcon.text = GameManager.GetPawnData(type).FriendlyName;
        }

        return pawnIcon;
    }

    public static ColorableIconData GetCheckedPlayer(ColorableIconData playerIcon, string playerName)
    {
        if (playerIcon.colored == null && playerIcon.baseIcon == null)
        {
            playerIcon = instance.fallbackPlayer;
            playerIcon.text = playerName;
        }

        return playerIcon;
    }    
}
