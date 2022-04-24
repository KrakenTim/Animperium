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
    [SerializeField] int amount = 1;

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
            newCell.SetResource(this);
    }

    public void Harvest()
    {
        GameManager.AddResource(Type, amount);

        SetHexCell(null);
        Destroy(gameObject);
    }
}
