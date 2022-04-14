using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        if (Mouse.current.leftButton.wasPressedThisFrame && MouseOverHexCheck.onHex)
        {
          ClickedOnHex();
        }
    }

    void ClickedOnHex()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
           selectedHexCell = HexGrid.GetHexCell(hit.point);

            Debug.Log("Change Selected Cell to "+ selectedHexCell.coordinates.ToString());
        }

        if (IsMovePossible())
        {
            InputMessage message = InputMessageGenerator.MoveToHex(selectedPawn, selectedHexCell);
            InputMessageExecuter.Send(message);
        }
    }

    private bool IsMovePossible()
    {
        return (selectedPawn != null && selectedPawn.CanAct && selectedHexCell != null 
            && selectedPawn.IsUnit && selectedPawn.MP > 0 
            && selectedPawn.IsPlayerPawn && !selectedHexCell.HasPawn
            && selectedHexCell.IsNeighbor(selectedPawn.HexCell));

    }

    private bool IsAttackPossible(PlayerPawn otherPawn)
    {
        return (selectedPawn != null&&selectedPawn.AttackPower > 0 && otherPawn != null && selectedPawn.CanAct
           && otherPawn.IsEnemy && selectedPawn.HexCell.IsNeighbor(otherPawn.HexCell));
    }

    public static void ClickedOnPawn(PlayerPawn clickedPawn)
    {
        if (instance.IsAttackPossible(clickedPawn))
        {
            InputMessage message = InputMessageGenerator.Attack(instance.selectedPawn, clickedPawn);
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
