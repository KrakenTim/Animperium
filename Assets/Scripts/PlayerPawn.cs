using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : MonoBehaviour
{
    [SerializeField] int playerID;
    [SerializeField] int factionID;

    [SerializeField] ePlayerPawnType type;
    public bool IsBuilding => type.IsBuilding();
    public bool IsUnit => type.IsUnit();

    [SerializeField] HexCell hexCell;

    public bool isActivePlayerPawn => playerID == GameManager.ActivePlayerID;

    public bool isEnemyPawn => factionID != GameManager.ActivePlayerFactionID;

    // Start is called before the first frame update
    void Start()
    {
        if (hexCell == null)
            SetHexCell(GameManager.GetHexCell(transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOwner(int playerID, int fractionID)
    {
        this.playerID = playerID;
        this.factionID = fractionID;
    }

    public void SetHexCell(HexCell cell)
    {
        hexCell = cell;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (hexCell)
            transform.position = hexCell.transform.position;
        else
            Debug.LogError("Tried to Update Position without HexCell", this);
    }
}
