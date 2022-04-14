using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceToken : MonoBehaviour
{
    [SerializeField] eRessourceType type;
    public eRessourceType Type => type;
    [SerializeField] int amount = 1;

    [SerializeField] HexCell hexCell;

    private void Awake()
    {
        if (Type == eRessourceType.NONE || amount < 1)
            Debug.LogError($"Ressource of type {type} with an amount of {amount} found!", this);
    }

    private void Start()
    {
        if (hexCell == null)
            SetHexCell(GameManager.GetHexCell(transform.position));
    }

    private void SetHexCell(HexCell newCell)
    {
        if(hexCell != null)
            hexCell.SetResource(null);

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
