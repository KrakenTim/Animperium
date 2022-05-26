using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Token to be placed on the grid field, harvest it to gain resources.
/// </summary>
public class RessourceToken : MonoBehaviour
{
    [SerializeField] eRessourceType type;
    public eRessourceType Type => type;
    [SerializeField] int amount = 10;

    [SerializeField] HexCell hexCell;
    public HexCell HexCell => hexCell;
    
    private void Start()
    {
        if (Type == eRessourceType.NONE || amount < 1)
            Debug.LogError($"Ressource of type {type} with an amount of {amount} found!", this);

        if (hexCell == null)
            SetHexCell(GameManager.GetHexCell(transform.position));
    }

    private void SetHexCell(HexCell newCell)
    {
        if(hexCell != null)
            hexCell.SetResource(null);

        hexCell = newCell;

        if (newCell != null)
        {
            newCell.SetResource(this);

            if (hexCell)
                transform.position = hexCell.transform.position;
        }
    }
    
    public void Harvest()
    {
        GameManager.AddResource(Type, amount);

        SetHexCell(null);
        Destroy(gameObject);
    }

    /// <summary>
    /// True if pawn can collect Ressource type.
    /// </summary>
    public bool CanCollect(PlayerPawn pawn)
    {
        switch (pawn.PawnType)
        {
            case ePlayerPawnType.Villager:
                if (type == eRessourceType.Ore)
                    return false;
                break;

            case ePlayerPawnType.Digger:
                if (type != eRessourceType.Ore)
                    return false;
                break;

            default:
                return false;
        }
        return true; //pawn.CanAct;
    }
}
