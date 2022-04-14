using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPawn : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] PlayerPawnData pawnData;
    public int MaxHealth => pawnData.maxHealth;
    public int MaxMovement => pawnData.maxMovement;
    public int AttackPower => pawnData.attackPower;
    public ePlayerPawnType PawnType => pawnData.type;
    public bool IsBuilding => PawnType.IsBuilding();
    public bool IsUnit => PawnType.IsUnit();

    [SerializeField] int playerID;
    public Sprite PlayerIcon => GameManager.GetPlayerIcon(playerID);
    public int PlayerID => playerID;
    [SerializeField] int factionID;


    [Space]
    [SerializeField] int currentHealth;
    public int HP => currentHealth;
    [SerializeField] int movementPoints;
    public int MP => movementPoints;

    [SerializeField] bool actedAlready = false;
    public bool CanAct => !actedAlready;





    [SerializeField] HexCell hexCell;
    public HexCell HexCell => hexCell;
    public HexCoordinates HexCoordinates => hexCell.coordinates;

    public bool IsPlayerPawn => playerID == GameManager.CurrentPlayerID;

    public bool IsEnemy => factionID != GameManager.CurrentFactionID;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = MaxHealth;
        movementPoints = MaxMovement;

        GameManager.AddPawn(this);

        if (hexCell == null)
            SetHexCell(GameManager.GetHexCell(transform.position));
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

    public void Attack(PlayerPawn victim)
    {
        actedAlready = true;
        PlayerHUD.UpdateShownPawn();
        victim.Damaged(AttackPower);
    }

    public void Collect(RessourceToken resource)
    {
        actedAlready = true;
        PlayerHUD.UpdateShownPawn();

        resource.Harvest();
    }

    public void MoveTo(HexCell targetPosition)
    {
        movementPoints -= 1;

        PlayerHUD.UpdateShownPawn();

        SetHexCell(targetPosition);
    }

    public void Damaged(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            GameManager.RemovePawn(this);

            SetHexCell(null);

            Destroy(gameObject);
        }
    }

    public void RefreshTurn()
    {
        movementPoints = MaxMovement;
        actedAlready = false;
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
        Debug.Log($"Clicked {PawnType} of Player {playerID} at HexPosition {hexCell.coordinates.ToString()}\n", this);
       
        GameInputManager.ClickedOnPawn(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerHUD.HoverPawn(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerHUD.UpdateShownPawn();
    }
}
