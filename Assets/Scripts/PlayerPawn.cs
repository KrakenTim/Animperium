using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPawn : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] int playerID;
    [SerializeField] int factionID;
    [SerializeField] int pawmHP;
    public int HP => pawmHP;
    [SerializeField] int pawnMP;
    public int MP => pawnMP;

    [SerializeField] ePlayerPawnType type;
    public ePlayerPawnType PawnType => type;
    public bool IsBuilding => type.IsBuilding();
    public bool IsUnit => type.IsUnit();

    [SerializeField] HexCell hexCell;
    public HexCoordinates HexCoordinates => hexCell.coordinates;

    public bool IsActivePlayerPawn => playerID == GameManager.ActivePlayerID;

    public bool IsEnemyPawn => factionID != GameManager.ActivePlayerFactionID;

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
        if (hexCell != null)
            hexCell.SetPawn(null);

        hexCell = cell;

        if (cell != null)
            cell.SetPawn(this);

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (hexCell)
            transform.position = hexCell.transform.position;
        else
            Debug.LogError("Tried to Update Position without HexCell", this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Clicked {type} of Player {playerID} at HexPosition {hexCell.coordinates.ToString()}\n", this);
       
        GameInputManager.ClickedOnPawn(this);
    }
}
