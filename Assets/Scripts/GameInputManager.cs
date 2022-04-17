using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public enum InputMode
    {
        NONE = 0,
        Movement = 1
    }

    static GameInputManager instance;

    public static HexGrid HexGrid => GameManager.HexGrid;

    public static event System.Action<PlayerPawn> SelectedPawn;

    public static event System.Action<InputMode> ChangedInputMode;

    [SerializeField] InputMode myInputMode;
    public static InputMode CurrentInputMode
    {
        get => instance.myInputMode;

        private set
        {
            instance.myInputMode = value;
            ChangedInputMode?.Invoke(value);
        }
    }

    [SerializeField] PlayerPawn selectedPawn;
    [SerializeField] HexCell selectedHexCell;


    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (MouseOverHexCheck.onHex && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ClickedOnHex();
        }
    }

    void ClickedOnHex()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(inputRay, out RaycastHit hit)) return;

        selectedHexCell = HexGrid.GetHexCell(hit.point);
        //Debug.Log($"InputSelection\tSelected Cell {selectedHexCell.coordinates.ToString()}\n", selectedHexCell);

        if (IsCollectPossible())
        {
            InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Collect);
            InputMessageExecuter.Send(message);
            return;
        }

        if (IsMovePossible())
        {
            InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Move);
            InputMessageExecuter.Send(message);
            return;
        }

        if (IsSpawnPossible())
        {
            InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Spawn);
            InputMessageExecuter.Send(message);
            return;
        }
    }

    private bool IsCollectPossible()
    {
        return selectedPawn != null && selectedHexCell != null && selectedPawn.CanAct
            && selectedPawn.IsUnit && selectedPawn.HexCell.IsNeighbor(selectedHexCell)
            && selectedHexCell.Resource != null;
    }

    private bool IsMovePossible()
    {
        return (selectedPawn != null && selectedPawn.CanAct && selectedHexCell != null
            && selectedPawn.IsUnit && selectedPawn.MP > 0
            && selectedPawn.IsPlayerPawn && !selectedHexCell.HasPawn
            && selectedHexCell.IsNeighbor(selectedPawn.HexCell));
    }

    private bool IsSpawnPossible()
    {
        return (selectedPawn != null && selectedHexCell != null
            && selectedPawn.IsPlayerPawn && !selectedHexCell.HasPawn
            && selectedHexCell.IsNeighbor(selectedPawn.HexCell)
            && selectedPawn.Spawn != ePlayerPawnType.NONE);
    }

    private bool IsAttackPossible(PlayerPawn otherPawn)
    {
        return (selectedPawn != null && selectedPawn.AttackPower > 0 && otherPawn != null && selectedPawn.CanAct
           && otherPawn.IsEnemy && selectedPawn.HexCell.IsNeighbor(otherPawn.HexCell));
    }

    public static void ClickedOnPawn(PlayerPawn clickedPawn)
    {
        if (instance.IsAttackPossible(clickedPawn))
        {
            InputMessage message = InputMessageGenerator.CreateHexMessage(instance.selectedPawn, clickedPawn.HexCell, ePlayeractionType.Attack);
            InputMessageExecuter.Send(message);
        }

        instance.selectedPawn = clickedPawn;

        // Debug.Log("Change Selected Cell to " + newlySelected.PawnType);

        SelectedPawn?.Invoke(clickedPawn);
    }

    public static void DeselectPawn()
    {
        instance.selectedPawn = null;
        SelectedPawn?.Invoke(null);
    }
}
