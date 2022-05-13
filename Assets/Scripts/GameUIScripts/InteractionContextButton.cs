using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionContextButton : MonoBehaviour
{
    [SerializeField] private ePlayerPawnType buttonPawn;

    private PlayerPawnData pawnData;
    [Space]
    [SerializeField] ColorableImage pawnIcon;
    [SerializeField] TMPro.TMP_Text pawnName;
    [Space]
    [SerializeField] TMPro.TMP_Text foodCost;
    [SerializeField] TMPro.TMP_Text woodCost;
    [SerializeField] TMPro.TMP_Text oreCost;

    private Button myButton;
    private PlayerPawnData upgradedUnit;
    private HexCell targetCell;
    public void Initialise(PlayerPawnData pawnData, HexCell targetCell, PlayerPawnData upgradedUnit = null)
    { 
        this.pawnData = pawnData;
        this.upgradedUnit = upgradedUnit;
        this.targetCell = targetCell;
        buttonPawn = pawnData.type;

        gameObject.name = buttonPawn + "InteractionContextButton";

        pawnIcon.SetPawn(pawnData, GameManager.LocalPlayerID);

        pawnName.text = pawnData.PawnName;

        if (myButton == null)
            myButton = GetComponent<Button>();

        if (upgradedUnit == null)
            SetCosts(pawnData.resourceCosts);
        else
            SetCosts(PawnUpgradeController.GetUpgradeCost(upgradedUnit, pawnData));
    }
    private void SetCosts(GameResources costs)
    {
        foodCost.text = costs.food.ToString();
        woodCost.text = costs.wood.ToString();
        oreCost.text = costs.ore.ToString();

        myButton.interactable = GameManager.CanAfford(GameManager.LocalPlayerID, costs);
    }

    public void Button_ButtonPressed()
    {
        Debug.Log($"InteractionButton\t {gameObject} wants to build a {pawnData.PawnName}. \n", gameObject);
        InteractionMenuManager.Close();

        InputMessage newMessage;

        if (upgradedUnit == null)
        newMessage = InputMessageGenerator.CreatePawnMessage(GameInputManager.SelectedPawn, 
                                                             targetCell, ePlayeractionType.Build, buttonPawn);
        else
            newMessage = InputMessageGenerator.CreatePawnMessage(GameInputManager.SelectedPawn,
                                                                 targetCell, ePlayeractionType.Learn, buttonPawn);
        InputMessageExecuter.Send(newMessage);
    }
}
