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


    [SerializeField] PlayerPawn currentlySelectedPawn;
    [SerializeField] HexCell currentlySelectedHexCell;


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
           currentlySelectedHexCell = HexGrid.GetHexCell(hit.point);

            Debug.Log("Change Selected Cell to "+ currentlySelectedHexCell.coordinates.ToString());
        }

        if (CheckMovementPossible())
        {
            InputMessage message = InputMessageGenerator.MoveToHex(currentlySelectedPawn, currentlySelectedHexCell);
            InputMessageExecuter.Send(message);
        }
    }

    private bool CheckMovementPossible()
    {
        return (currentlySelectedPawn != null && currentlySelectedHexCell != null 
            && currentlySelectedPawn.IsUnit && currentlySelectedPawn.MP > 0 
            && currentlySelectedPawn.IsActivePlayerPawn);
    }

    public static void ClickedOnPawn(PlayerPawn newlySelected)
    {
        instance.currentlySelectedPawn = newlySelected;

        Debug.Log("Change Selected Cell to " + newlySelected.PawnType);

        SelectedPawn?.Invoke(newlySelected);
    }
}
