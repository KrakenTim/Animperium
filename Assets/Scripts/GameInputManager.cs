using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the player's input.
/// </summary>
public class GameInputManager : MonoBehaviour
{
    static GameInputManager instance;

    public static HexGrid HexGrid => GameManager.HexGrid;

    public static event System.Action<PlayerPawn> SelectedPawn;

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
    private void Update()
    {
        if (MouseOverHexCheck.IsOnHex && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ClickedOnHex();
        }
    }

    private void ClickedOnHex()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(inputRay, out RaycastHit hit)) return;

        selectedHexCell = HexGrid.GetHexCell(hit.point);

        if (IsPawnActionPossible(selectedHexCell))
        {
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
    }

    /// <summary>
    /// Checks if selected Pawn is Player Pawn, next to selected Cell and can act
    /// </summary>
    private bool IsPawnActionPossible(HexCell targetCell)
    {
        return selectedPawn != null && targetCell != null
            && selectedPawn.CanAct && selectedPawn.IsPlayerPawn
            && selectedPawn.HexCell.IsNeighbor(targetCell);
    }

    private bool IsCollectPossible()
    {
        return selectedPawn.IsUnit && selectedHexCell.Resource != null && selectedPawn.MP > 0;
    }

    private bool IsMovePossible()
    {
        return selectedPawn.IsUnit && selectedPawn.MP > 0
            && !selectedHexCell.HasPawn;
    }

    private bool IsSpawnPossible()
    {
        // TODO(24.04.22) might check if needed resources are available.
        return !selectedHexCell.HasPawn
               && selectedPawn.Spawn != ePlayerPawnType.NONE;
    }

    private bool IsAttackPossible(PlayerPawn otherPawn)
    {
        return selectedPawn.AttackPower > 0 && otherPawn != null
           && otherPawn.IsEnemy;
    }

    private bool IsLearningPossible(PlayerPawn potentialSchool)
    {
        if (potentialSchool.PlayerID == selectedPawn.PlayerID && potentialSchool.PawnType.IsSchool()
            && PawnUpgradeController.TryUpgradePossible(selectedPawn.PawnType, potentialSchool.PawnType.Teaches(),
                                                     selectedPawn.PlayerID, out PlayerPawnData newUnit, out GameResources costs))
            return true;

        return false;
    }

    /// <summary>
    /// Called if a pawn was clicked.
    /// </summary>
    public static void ClickedOnPawn(PlayerPawn clickedPawn)
    {
        // Check if enemy that might be attacked
        if (instance.IsPawnActionPossible(clickedPawn.HexCell) && instance.IsAttackPossible(clickedPawn))
        {
            InputMessage message = InputMessageGenerator.CreateHexMessage(instance.selectedPawn, clickedPawn.HexCell,
                                                                          ePlayeractionType.Attack);
            InputMessageExecuter.Send(message);
        }

        // Check if school and upgrade possible
        if (instance.IsPawnActionPossible(clickedPawn.HexCell) && instance.IsLearningPossible(clickedPawn))
        {
            InputMessage message = InputMessageGenerator.CreateHexMessage(instance.selectedPawn, clickedPawn.HexCell,
                                                                          ePlayeractionType.Learn);
            InputMessageExecuter.Send(message);
        }

        // Select clicked Pawn
        instance.selectedPawn = clickedPawn;

        SelectedPawn?.Invoke(clickedPawn);
    }

    public static void DeselectPawn()
    {
        instance.selectedPawn = null;
        SelectedPawn?.Invoke(null);
    }
}
