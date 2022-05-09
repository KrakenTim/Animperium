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

    public static event System.Action<PlayerPawn> SelectPawn;

    [SerializeField] PlayerPawn selectedPawn;
    public static PlayerPawn SelectedPawn => instance.selectedPawn;
    [SerializeField] HexCell selectedHexCell;
    public static HexCell SelectedHexCell => instance.selectedHexCell;

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
        if (MouseOverHexCheck.IsOnHex)
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClickedOnHex(wasLeftClick: true);
            }
            else if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                ClickedOnHex(wasLeftClick: false);
            }
        }
    }

    private void ClickedOnHex(bool wasLeftClick)
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(inputRay, out RaycastHit hit)) return;

        selectedHexCell = HexGrid.GetHexCell(hit.point);

        if (IsPawnActionPossible(selectedHexCell))
        {
            if (wasLeftClick)
            {
                if (IsCollectPossible())
                {
                    InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Collect);
                    InputMessageExecuter.Send(message);
                    return;
                }

                if (IsSpawnPossible())
                {
                    InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Spawn);
                    InputMessageExecuter.Send(message);
                    return;
                }

                if (IsBuildingPossible())
                InteractionMenuManager.OpenPawnCreationMenu(selectedHexCell);

            }
            else // rightClick
            {
                if (IsMovePossible())
                {
                    InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Move);
                    InputMessageExecuter.Send(message);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Checks if selected Pawn is Player Pawn, next to selected Cell and can act
    /// </summary>
    private bool IsPawnActionPossible(HexCell targetCell)
    {
        return GameManager.InputAllowed && selectedPawn != null && targetCell != null
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

    private bool IsBuildingPossible()
    {
        return selectedPawn.PawnType == ePlayerPawnType.Villager && !selectedHexCell.HasPawn;
    }

    private bool IsAttackPossible(PlayerPawn otherPawn)
    {
        return selectedPawn.AttackPower > 0 && otherPawn != null
           && otherPawn.IsEnemy;
    }

    private bool IsLearningPossible(PlayerPawn potentialSchool)
    {
        if (potentialSchool.PlayerID == selectedPawn.PlayerID && potentialSchool.PawnType.IsSchool()
            && selectedPawn.PawnData.IsUpgradePossible)
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
            InteractionMenuManager.OpenPawnCreationMenu(clickedPawn.HexCell, instance.selectedPawn.PawnData);
            return;
        }

        // Select clicked Pawn
        instance.selectedPawn = clickedPawn;

        SelectPawn?.Invoke(clickedPawn);
    }

    public static void DeselectPawn()
    {
        instance.selectedPawn = null;
        SelectPawn?.Invoke(null);
    }
}
