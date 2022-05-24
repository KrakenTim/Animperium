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
    [SerializeField] Image notPossibleBackground;

    private Button myButton;
    private PlayerPawnData actingUnit;
    private HexCell targetCell;
    private ePlayeractionType actionType;

    public void Initialise(PlayerPawnData newPawnData, HexCell targetCell, PlayerPawnData actingUnit, ePlayeractionType actionType,
                           Color playerColor)
    {
        this.newPawnData = newPawnData;
        this.actingUnit = actingUnit;
        this.targetCell = targetCell;
        this.actionType = actionType;

        gameObject.name = newPawnData.type + "InteractionContextButton";

        pawnIcon.SetPawn(newPawnData, GameManager.LocalPlayerID);

        pawnName.text = newPawnData.friendlyName;

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
                CheckUpgradeUnlocked(newPawnData);
                break;
            case ePlayeractionType.BuildingUpgrade:
                // get upgrade cost for building on target cell
                SetCosts(PawnUpgradeController.GetUpgradeCost(targetCell.Pawn.PawnData, newPawnData));
                CheckUpgradeUnlocked(newPawnData);
                break;

            default:
                Debug.LogError("ContextButton\tInitialised UNDEFINED for " + actionType);
                break;
        }

        if (!myButton.interactable)
            notPossibleBackground.color = playerColor;
    }

    /// <summary>
    /// Sets ressources according to needs, prepares NotPossible if the player doesn't have enough.
    /// </summary>
    private void SetCosts(GameResources costs)
    {
        foodCost.text = costs.food.ToString();
        woodCost.text = costs.wood.ToString();
        oreCost.text = costs.ore.ToString();

        if (!GameManager.CanAfford(GameManager.LocalPlayerID, costs))
            PrepareNotPossible("Not enough Resources!");
    }

    /// <summary>
    /// Checks if the pawnData is unlocked, prepares NotPossible otherwise.
    /// </summary>
    private void CheckUpgradeUnlocked(PlayerPawnData pawnUpgrade)
    {
        int neededUpgrades = pawnUpgrade.upgradesForUnlock - GameManager.GetUpgradeCount(GameManager.LocalPlayerID);
        if (neededUpgrades > 0)
        {
            if (neededUpgrades > 1)
                PrepareNotPossible($"Need to upgrade {neededUpgrades} more units!");
            else
                PrepareNotPossible($"Need to upgrade one more unit!");
        }
    }

    public void Button_ButtonPressed()
    {
        Debug.Log($"InteractionButton\t {gameObject} wants to build a {newPawnData.friendlyName}. \n", gameObject);
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

    private void PrepareNotPossible(string messageText)
    {
        notPossibleMessage.text = messageText;
        myButton.interactable = false;
    }
}
