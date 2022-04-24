using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        return selectedPawn.IsUnit  && selectedHexCell.Resource != null && selectedPawn.MP > 0;
    }

    private bool IsMovePossible()
    {
        return selectedPawn.IsUnit && selectedPawn.MP > 0
            && !selectedHexCell.HasPawn;
    }

    private bool IsSpawnPossible()
    {
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
            || PawnUpgradeController.TryUpgradePossible(selectedPawn.PawnType, potentialSchool.PawnType.Teaches(),
                                                        selectedPawn.PlayerID, out PlayerPawnData newUnit))
            return true;

        return false;
    }

    public static void ClickedOnPawn(PlayerPawn clickedPawn)
    {
        if (instance.IsPawnActionPossible(clickedPawn.HexCell) && instance.IsAttackPossible(clickedPawn))
        {
            InputMessage message = InputMessageGenerator.CreateHexMessage(instance.selectedPawn, clickedPawn.HexCell, ePlayeractionType.Attack);
            InputMessageExecuter.Send(message);
        }

        if (instance.IsPawnActionPossible(clickedPawn.HexCell) && instance.IsLearningPossible(clickedPawn))
        {
            InputMessage message = InputMessageGenerator.CreateHexMessage(instance.selectedPawn, clickedPawn.HexCell, ePlayeractionType.Learn);
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
