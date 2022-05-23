using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[System.Serializable]
public class PlayerPawn : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] PlayerPawnData pawnData;
    public PlayerPawnData PawnData => pawnData;
    public ePlayerPawnType PawnType => pawnData.type;
    public int MaxHealth => pawnData.maxHealth;
    public int MaxMovement => pawnData.maxMovement;
    public int AttackPower => pawnData.attackPower;
    public int AttackRange => pawnData.attackRange;
    public int Population => pawnData.population;
    public ePlayerPawnType Spawn => pawnData.spawnedPawn;
    public bool CanHeal => (PawnType == ePlayerPawnType.Healer);
    public bool IsMagicUser => (PawnType == ePlayerPawnType.Warmage || PawnType == ePlayerPawnType.Healer);
    public bool IsBuilding => PawnType.IsBuilding();
    public bool IsUnit => PawnType.IsUnit();

    /// <summary>
    /// Returns Pawn Icon if not null else it's the Players Icon
    /// </summary>
    public ColorableIconData PawnIcon => pawnData.PawnIcon;
    /// <summary>
    /// Returns the players individual icon.
    /// </summary>
    public ColorableIconData PlayerIcon => GameManager.GetPlayerIcon(playerID);

    [SerializeField] int playerID;
    public int PlayerID => playerID;

    /// <summary>
    /// used to more easily track pawns which are otherwise similar.
    /// </summary>
    [HideInInspector] public int pawnID = 0;
    public string FriendlyName => PawnData.friendlyName;

    [Space]
    [SerializeField] int currentHealth;
    public int HP => currentHealth;
    public bool IsWounded => currentHealth < MaxHealth;
    public HealthBar healthBar { get; private set; }
    
    [SerializeField] int movementPoints;
    public int MP => movementPoints;

    bool _canAct = true;
    /// <summary>
    /// True if Pawn can act, setting it to false sets movement points to zero.
    /// </summary>
    public bool CanAct
    {
        get => _canAct;
        private set
        {
            _canAct = value;

            if (!_canAct)
            {
                movementPoints = 0;
                PlayerHUD.UpdateShownPawn();
            }
        }
    }
    [Space]
    [SerializeField] HexCell hexCell;
    public HexCell HexCell => hexCell;
    public HexCoordinates HexCoordinates => hexCell ? hexCell.coordinates : new HexCoordinates(0, 0);
    public Vector3 WorldPosition => transform.position;
    public float RotationY
    {
        get => transform.eulerAngles.y;
        set
        {
            Vector3 euler = transform.eulerAngles;
            euler.y = value;
            transform.eulerAngles = euler;
        }
    }

    public virtual bool IsPlayerPawn => playerID == GameManager.ActivePlayerID;

    public virtual bool IsEnemy => GameManager.IsEnemy(PlayerID);

    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = MaxHealth;
        movementPoints = MaxMovement;

        if (!PawnType.IsNonPlayer())
            GameManager.AddPlayerPawn(this);

        if (hexCell == null)
            SetHexCell(GameManager.GetHexCell(transform.position));

        healthBar = HealthbarManager.AddHealthbar(this);
    }

    private void OnDestroy()
    {
        HealthbarManager.RemoveHealthbar(this);
    }

    /// <summary>
    /// Set the changing variable values of the pawn (HP, MP, etc.).
    /// </summary>
    public void Initialize(int playerID, int hp, int mp, bool canAct)
    {
        SetPlayer(playerID);
        currentHealth = hp;
        movementPoints = mp;
        this.CanAct = canAct;
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

    /// <summary>
    /// Turns the character to look away from the given cell
    /// </summary>
    public void LookAway(HexCell cellInBack)
    {
        Vector3 target = transform.position + (transform.position - cellInBack.Position);
        target.y = transform.position.y;

        LookAt(target);
    }

    public void LookAt(Vector3 worldPosition)
    {
        worldPosition.y = transform.position.y;
        transform.LookAt(worldPosition);
    }

    public bool CanLearn(eKnowledge newKnowledge, out ePlayerPawnType newType)
    {
        return pawnData.CanLearn(newKnowledge, out newType);
    }

    public bool InAttackRange(PlayerPawn otherPawn)
    {
        return HexCell.DistanceTo(otherPawn.HexCell) <= pawnData.attackRange;
    }

    public void Attack(PlayerPawn victim)
    {
        if (PawnType == ePlayerPawnType.Blaster && victim.IsBuilding)
            victim.Damaged(this, Mathf.Max(AttackPower, pawnData.specialPower));
        else
            victim.Damaged(this, AttackPower);

        CanAct = false;
    }

    public void Collect(RessourceToken resource)
    {
        MoveTo(resource.HexCell);
        CanAct = false;

        resource.Harvest();
    }

    public void Build(HexCell targetCell, ePlayerPawnType buildUnit)
    {
        if (!GameManager.SpawnPawn(this, targetCell, buildUnit)) return;

        CanAct = false;
    }

    public void UpgradedBuilding(PlayerPawn upgradeBuilding)
    {
        CanAct = false;
    }

    public void MoveTo(HexCell targetPosition)
    {
        movementPoints -= 1;

        PlayerHUD.UpdateShownPawn();

        // rotates pawn according to direction it came from
        HexCell oldPosition = hexCell;

        SetHexCell(targetPosition);
        LookAway(oldPosition);
    }

    public void Damaged(PlayerPawn attacker, int damageAmount)
    {
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        healthBar?.UpdateHealthBar();

        if (currentHealth <= 0)
        {
            FeedbackManager.PlayPawnDestroyed(this);

            GameManager.RemovePlayerPawn(this);
        }
        else
        {
            FeedbackManager.PlayPawnDamaged(this);
            LookAt(attacker.WorldPosition);
        }
    }

    public void HealTarget(PlayerPawn healTarget)
    {
        if (!healTarget.IsWounded || healTarget.IsEnemy) return;

        healTarget.GetHealed(pawnData.specialPower);

        CanAct = false;
    }

    public void GetHealed(int healedAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healedAmount, MaxHealth);

        healthBar?.UpdateHealthBar();
    }

    public void RefreshTurn()
    {
        movementPoints = MaxMovement;
        CanAct = true;
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

    public void UpdateHexCellViaEditor()
    {
        HexCell below = GameManager.GetHexCell(transform.position);

        if (below.HasPawn)
        {
            Debug.Log($"PlayerPawn\tTwo Pawns on the same position.\n\t\t{below.Pawn.ToString()}\n\t\t{this.ToString()}\n");
            return;
        }

        SetHexCell(below);
        Debug.Log($"PlayerPawn\tPlaced {PawnType}({WorldPosition}).\n\t\t{ToString()}\n");
    }
}

#if UNITY_EDITOR
/// <summary>
/// Extends the default Unity Editor for the Class.
/// </summary>
[CustomEditor(typeof(PlayerPawn))]
public class PlayerPawnEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Set Pawn to Hex"))
            ((PlayerPawn)target).UpdateHexCellViaEditor();
        base.OnInspectorGUI();
    }
}
#endif // UNITY_EDITOR