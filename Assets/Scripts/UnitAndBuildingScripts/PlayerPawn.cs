using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class PlayerPawn : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    const int FoodPerTurn = 5;
    const int WoodPerTurn = 5;
    const string PawnAnimationAttacking = "Attacking";
    const string PawnAnimationDefeated = "Defeated";

    public System.Action OnValueChange;

    /// <summary>
    /// Called after Pawn was moved, Parameters: moved Pawn, old HexCell, new HexCell.
    /// </summary>
    public static event System.Action<PlayerPawn, HexCell, HexCell> OnPawnMoved;

    [SerializeField] PlayerPawnData pawnData;
    [SerializeField] Animator animator;
    public PlayerPawnData PawnData => pawnData;
    public ePlayerPawnType PawnType => pawnData.type;
    public int MaxHealth => pawnData.maxHealth;
    public int MaxMovement => pawnData.maxMovement;
    public int AttackPower => pawnData.attackPower;
    public int AttackRange => pawnData.attackRange;
    public ePlayerPawnType Spawn => pawnData.spawnedPawn;

    public bool CanHeal => (PawnType == ePlayerPawnType.Healer);
    public bool CanStealth => (PawnType == ePlayerPawnType.Sneaker || PawnType == ePlayerPawnType.TunnelEntry);
    public bool CanDig => (PawnType == ePlayerPawnType.Digger || PawnType == ePlayerPawnType.Blaster);

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
    public string FriendlyName => PawnData.FriendlyName;

    [Space]
    [SerializeField] int currentHealth;
    public int HP => currentHealth;
    public bool IsWounded => currentHealth < MaxHealth;
    public HealthBar healthBar { get; private set; }

    [SerializeField] bool isStealthed;
    public bool IsStealthed => isStealthed;

    [SerializeField] int movementPoints;
    public int MP => movementPoints;

    bool _canAct = true;

    float jumpHeight = 5f;
    float jumpTime = 0.25f;

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
                movementPoints = 0;

            OnValueChange?.Invoke();
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

    public virtual bool IsActivePlayerPawn => playerID == GameManager.ActivePlayerID;
    public virtual bool IsEnemyOf(int otherPlayerID) => PlayerValueProvider.AreEnemies(PlayerID, otherPlayerID);
    public virtual bool IsEnemyOf(PlayerPawn otherPawn) => PlayerValueProvider.AreEnemies(PlayerID, otherPawn.PlayerID);

    protected virtual void Awake()
    {
        if (!GameManager.InGame) return;

        currentHealth = MaxHealth;
        movementPoints = MaxMovement;

        if (!PawnType.IsNonPlayer())
            GameManager.AddPlayerPawn(this);

        if (hexCell == null)
            SetHexCell(HexGridManager.Current.GetHexCell(transform.position));

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
        
        OnValueChange?.Invoke();

        if (currentHealth < MaxHealth)
            healthBar.UpdateHealthBar();
    }

    public void SetPlayer(int playerID)
    {
        this.playerID = playerID;
    }

    public void SetHexCell(HexCell newCell)
    {
        if (hexCell != null)
            hexCell.SetPawn(null);

        HexCell oldCell = hexCell;
        hexCell = newCell;

        if (newCell != null)
        {
            newCell.SetPawn(this);

            UpdatePosition();
        }

        OnPawnMoved?.Invoke(this, oldCell, newCell);
    }

    /// <summary>
    /// Turns the character to look away from the given cell
    /// </summary>
    public void LookAway(HexCell cellInBack)
    {
        Vector3 target = transform.position + (transform.position - cellInBack.transform.position);
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

        if (animator)
        {
            animator.SetTrigger(PawnAnimationAttacking);
        }

        CanAct = false;
    }

    public void Collect(ResourceToken resource)
    {
        MoveTo(resource.HexCell);
        CanAct = true;

        resource.Harvest();
    }

    public void Build(HexCell targetCell, ePlayerPawnType buildUnit)
    {
        if (!GameManager.SpawnPawn(this, targetCell, buildUnit)) return;

        CanAct = false;
    }

    public void UpgradedBuilding(PlayerPawn upgradeBuilding)
    {
        CanAct = true;
    }

    public void MoveTo(HexCell targetPosition)
    {
        movementPoints -= 1;

        HexCell oldPosition = hexCell;

        SetHexCell(targetPosition);

        // rotates pawn according to direction it came from
        LookAway(oldPosition);

        OnValueChange?.Invoke();

        if (oldPosition.gridLayer == targetPosition.gridLayer)
            StartCoroutine(JumpTo(oldPosition.transform.position, targetPosition.ObjectPosition, jumpTime));
    }

    public void Dig(HexCell targetCell)
    {
        if (!HexGridManager.Current.DigAwayCell(targetCell)) return;

        if (HexGridManager.IsWalkable(targetCell))
            MoveTo(targetCell);
    }

    public void SwitchLayer(PlayerPawn usedTunnel)
    {
        MoveTo(HexGridManager.Current.OtherLayerCell(HexCell));
    }

    public virtual void Damaged(PlayerPawn attacker, int damageAmount)
    {
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        healthBar?.UpdateHealthBar();

        if (currentHealth <= 0)
        {
            if (animator)
            {
                animator.SetTrigger(PawnAnimationDefeated);
            }

            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            FeedbackManager.PlayPawnDestroyed(this);

            GameManager.RemovePlayerPawn(this, waitBeforeDestroy : true);
        }
        else
        {
            FeedbackManager.PlayPawnDamaged(this);
            if (IsUnit)
                LookAt(attacker.WorldPosition);
        }

        OnValueChange?.Invoke();
    }

    public void HealTarget(PlayerPawn healTarget)
    {
        if (!healTarget.IsWounded || IsEnemyOf(healTarget)) return;

        healTarget.GetHealed(pawnData.specialPower);

        CanAct = false;
    }

    public void GetHealed(int healedAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healedAmount, MaxHealth);

        healthBar?.UpdateHealthBar();

        OnValueChange?.Invoke();
    }

    public void RefreshTurn()
    {
        movementPoints = MaxMovement;
        CanAct = true;

        if (PawnType == ePlayerPawnType.FarmHouse && GameManager.ActivePlayerID == PlayerID)
            GameManager.AddResource(eResourceType.Food, FoodPerTurn);

        if (PawnType == ePlayerPawnType.SawMill && GameManager.ActivePlayerID == PlayerID)
            GameManager.AddResource(eResourceType.Wood, WoodPerTurn);

    }

    public void UpdatePosition()
    {
        if (hexCell)
            transform.position = hexCell.ObjectPosition;
        else
            Debug.LogError("Tried to Update Position without HexCell", this);
    }

    public void SetStealthed(bool isHidden)
    {
        // Debug.Log($"Set Stealthed of {this} to {isHidden}\n");

        if (isHidden == isStealthed) return;

        isStealthed = isHidden;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        Debug.Log($"Update Visuals of {gameObject.name}, stealthed: {isStealthed}\n");

        if (IsStealthed && IsEnemyOf(GameManager.LocalPlayerID))
        {
            SetVisible(false);
            return;
        }
        else
            SetVisible(true);
    }

    private void SetVisible(bool isShown)
    {
        gameObject.SetActive(isShown);
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
        PlayerHUD.UnHoverPawn();
    }

    public override string ToString()
    {
        return $"{gameObject.name}[{PawnType},Player:{PlayerID}, Position:{HexCoordinates},{HP}HP, {MP}MP, CanAct:{CanAct}]";
    }

    public void UpdateHexCellViaEditor()
    {
        HexCell below = HexGridManager.Current.GetHexCell(transform.position);

        if (below.HasPawn)
        {
            Debug.Log($"PlayerPawn\tTwo Pawns on the same position.\n\t\t{below.Pawn.ToString()}\n\t\t{this.ToString()}\n");
            return;
        }

        SetHexCell(below);
        Debug.Log($"PlayerPawn\tPlaced {PawnType}({WorldPosition}).\n\t\t{ToString()}\n");
    }

    IEnumerator JumpTo(Vector3 startPosition, Vector3 targetPosition, float time)
    {
        float elapsed = 0;
        float progress;
        Vector3 currentPosition;

        while (elapsed < time)
        {
            progress = Mathf.SmoothStep(0f, 1f, elapsed / time);

            currentPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            currentPosition.y += Mathf.Sin(Mathf.PI * elapsed / time) * jumpHeight;

            transform.position = currentPosition;

            elapsed += Time.deltaTime;
            yield return null;
        }

        UpdatePosition();
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