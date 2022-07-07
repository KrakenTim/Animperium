using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Token to be placed on the grid field, harvest it to gain resources.
/// </summary>
public class ResourceToken : MonoBehaviour
{
    [SerializeField] eResourceType type;
    public eResourceType Type => type;
    [SerializeField] int amount = 10;

    [SerializeField] HexCell hexCell;
    public HexCell HexCell => hexCell;
    
    private void Start()
    {
        if (!GameManager.InGame) return;

        if (Type == eResourceType.NONE || amount < 1)
            Debug.LogError($"Ressource of type {type} with an amount of {amount} found!", this);

        if (hexCell == null)
            SetHexCell(HexGridManager.Current.GetHexCell(transform.position));
        
        if (type == eResourceType.Ore)
        {
            PlayerPawn.OnPawnMoved += UpdateVisibility;
            GameManager.LocalPlayerChanged += UpdateVisibility;

            UpdateVisibility();
        }
    }

    private void OnDestroy()
    {
        if (type == eResourceType.Ore)
        {
            PlayerPawn.OnPawnMoved -= UpdateVisibility;
            GameManager.LocalPlayerChanged -= UpdateVisibility;
        }
    }

    public void SetHexCell(HexCell newCell)
    {
        if (hexCell != null)
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
                if (type == eResourceType.Ore)
                    return false;
                break;

            case ePlayerPawnType.Digger:
                if (type != eResourceType.Ore)
                    return false;
                break;

            default:
                return false;
        }
        return true; //pawn.CanAct;
    }

    private void UpdateVisibility()
    {
        SetVisible(HasAlliedNeighbours());
    }

    /// <summary>
    /// TODO (24.06.2022) OPTIMIZE!
    /// </summary>
    private void UpdateVisibility(PlayerPawn pawn, HexCell oldCell, HexCell newCell)
    {
        if (pawn.IsEnemyOf(GameManager.LocalPlayerID)) return;

        if (newCell && newCell.DistanceTo(HexCell) <= 1 && newCell.gridLayer == hexCell.gridLayer)
        {
            SetVisible(true);
            return;
        }

        if (oldCell && oldCell.DistanceTo(HexCell) <= 1 && oldCell.gridLayer == hexCell.gridLayer)
        {
            SetVisible(HasAlliedNeighbours());
        }
    }

    private bool HasAlliedNeighbours()
    {
        HashSet<HexCell> neighbours = HexGridManager.Current.GetGrid(hexCell).GetNeighbours(hexCell, 1);

        foreach (var cell in neighbours)
        {
            if (cell.Pawn && !cell.Pawn.IsEnemyOf(GameManager.LocalPlayerID))
            {
                return true;
            }
        }
        return false;
    }

    private void SetVisible(bool IsVisible)
    {
        gameObject.SetActive(IsVisible);
    }
}
