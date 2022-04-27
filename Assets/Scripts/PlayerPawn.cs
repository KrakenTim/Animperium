using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class PlayerPawn : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] PlayerPawnData pawnData;

    public ePlayerPawnType PawnType => pawnData.type;
    public int MaxHealth => pawnData.maxHealth;
    public int MaxMovement => pawnData.maxMovement;
    public int AttackPower => pawnData.attackPower;
    public ePlayerPawnType Spawn => pawnData.spawnedPawn;

    public bool IsBuilding => PawnType.IsBuilding();
    public bool IsUnit => PawnType.IsUnit();

    /// <summary>
    /// Returns Pawn Icon if not null else it's the Players Icon
    /// </summary>
    public Sprite PawnIcon => (pawnData.pawnIcon != null) ? pawnData.pawnIcon : GameManager.GetPlayerIcon(PlayerID);
    /// <summary>
    /// Returns the players individual icon.
    /// </summary>
    public Sprite PlayerIcon => GameManager.GetPlayerIcon(playerID);

    [SerializeField] int playerID;
    public int PlayerID => playerID;

    [Space]
    [SerializeField] int currentHealth;
    public int HP => currentHealth;
    [SerializeField] int movementPoints;
    public int MP => movementPoints;

    [SerializeField] bool actedAlready = false;
    public bool CanAct => !actedAlready;
    [Space]
    [SerializeField] HexCell hexCell;
    public HexCell HexCell => hexCell;
    public HexCoordinates HexCoordinates => hexCell ? hexCell.coordinates : new HexCoordinates(0, 0);

    public virtual bool IsPlayerPawn => playerID == GameManager.CurrentPlayerID;

    public virtual bool IsEnemy => GameManager.IsEnemy(PlayerID);

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = MaxHealth;
        movementPoints = MaxMovement;

        if (!PawnType.IsNonPlayer())
            GameManager.AddPlayerPawn(this);

        if (hexCell == null)
            SetHexCell(GameManager.GetHexCell(transform.position));

        Debug.Log(ToString());
    }

    /// <summary>
    /// Set the changing variable values of the pawn (HP, MP, etc.).
    /// </summary>
    public void Initialize(int playerID, int hp, int mp, bool canAct)
    {
        SetPlayer(playerID);
        currentHealth = hp;
        movementPoints = mp;
        actedAlready = canAct;
    }

    public void SetPlayer(int playerID)
    {
        this.playerID = playerID;
    }

    public void SetHexCell(HexCell cell)
    {
        if (hexCell != null)
            hexCell.SetPawn(null);

        hexCell = cell;

        if (cell != null)
        {
            cell.SetPawn(this);
            UpdatePosition();
        }
    }

    public bool CanLearn(eKnowledge newKnowledge, out ePlayerPawnType newType)
    {
        return pawnData.CanLearn(newKnowledge, out newType);
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

        MoveTo(resource.HexCell);

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

            GameManager.CheckIfGameEnds(PlayerID);
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

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"PlayerPawn\tClicked {PawnType}[P{playerID}]\n\t\t{hexCell.coordinates.ToString()}\n", this);

        GameInputManager.ClickedOnPawn(this);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        PlayerHUD.HoverPawn(this);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        PlayerHUD.UpdateShownPawn();
    }

    public override string ToString()
    {
        return $"{gameObject.name}[{PawnType},Player:{PlayerID}, Position:{HexCoordinates},{HP}HP, {MP}MP, CanAct:{CanAct}]";
    }
}
