using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionContextButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerPawnData newPawnData;
    [Space]
    [SerializeField] ColorableImage pawnIcon;
    [SerializeField] TMPro.TMP_Text pawnName;
    [Space]
    [SerializeField] TMPro.TMP_Text foodCost;
    [SerializeField] TMPro.TMP_Text woodCost;
    [SerializeField] TMPro.TMP_Text oreCost;

    [Space]
    [SerializeField] GameObject notPossibleBox;
    [SerializeField] TMPro.TMP_Text notPossibleMessage;

    private Button myButton;
    private PlayerPawnData actingUnit;
    private HexCell targetCell;
    private ePlayeractionType actionType;
    public void Initialise(PlayerPawnData newPawnData, HexCell targetCell, PlayerPawnData actingUnit, ePlayeractionType actionType)
    {
        this.newPawnData = newPawnData;
        this.actingUnit = actingUnit;
        this.targetCell = targetCell;
        this.actionType = actionType;

        gameObject.name = newPawnData.type + "InteractionContextButton";

        pawnIcon.SetPawn(newPawnData, GameManager.LocalPlayerID);

        pawnName.text = newPawnData.PawnName;

        if (myButton == null)
            myButton = GetComponent<Button>();

        notPossibleBox.SetActive(false);

        switch (actionType)
        {
            case ePlayeractionType.Spawn:
            case ePlayeractionType.Build:
                SetCosts(newPawnData.resourceCosts);
                break;
            case ePlayeractionType.UnitUpgrade:
                SetCosts(PawnUpgradeController.GetUpgradeCost(actingUnit, newPawnData));
                break;
            case ePlayeractionType.BuildingUpgrade:
                // get upgarde cost for building on target cell
                SetCosts(PawnUpgradeController.GetUpgradeCost(targetCell.Pawn.PawnData, newPawnData));

                int neededUpgrades = GameManager.SchoolNeededUpgrades - GameManager.GetUpgradeCount(GameManager.LocalPlayerID);
                myButton.interactable = neededUpgrades <= 0;
                if (!myButton.interactable)
                    notPossibleMessage.text = $"Need to Upgrade {neededUpgrades} more Upgraded units.";
                break;

            default:
                Debug.LogError("ContextButton\tInitialised UNDEFINED for " + actionType);
                break;
        }
    }
    private void SetCosts(GameResources costs)
    {
        foodCost.text = costs.food.ToString();
        woodCost.text = costs.wood.ToString();
        oreCost.text = costs.ore.ToString();

        myButton.interactable = GameManager.CanAfford(GameManager.LocalPlayerID, costs);

        if (!myButton.interactable)
            notPossibleMessage.text = "Not enough Resources!";
    }

    public void Button_ButtonPressed()
    {
        Debug.Log($"InteractionButton\t {gameObject} wants to build a {newPawnData.PawnName}. \n", gameObject);
        InteractionMenuManager.Close();

        InputMessage newMessage = InputMessageGenerator.CreatePawnMessage(GameInputManager.SelectedPawn,
                                                              targetCell, actionType, newPawnData.type);
        InputMessageExecuter.Send(newMessage);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!myButton.interactable)
            notPossibleBox.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        notPossibleBox.SetActive(false);
    }
}
