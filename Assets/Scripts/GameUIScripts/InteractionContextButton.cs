using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionContextButton : MonoBehaviour
{
    [SerializeField] private ePlayerPawnType buttonPawn;

    private PlayerPawnData pawnData;
    [Space]
    [SerializeField] Image pawnIcon;
    [SerializeField] TMPro.TMP_Text pawnName;
    [Space]
    [SerializeField] TMPro.TMP_Text foodCost;
    [SerializeField] TMPro.TMP_Text woodCost;
    [SerializeField] TMPro.TMP_Text oreCost;

    private Button myButton;

    public void Initialise(PlayerPawnData pawnData)
    { 
        this.pawnData = pawnData;
        buttonPawn = pawnData.type;

        gameObject.name = buttonPawn + "InteractionContextButton";

        pawnIcon.sprite = pawnData.pawnIcon;
        if (pawnIcon.sprite == null)
            pawnIcon.sprite = GameManager.GetPlayerIcon(GameManager.LocalPlayerID);
        pawnName.text = pawnData.PawnName;

        SetCosts(pawnData.resourceCosts);
    }
    public void SetCosts(GameResources costs)
    {
        foodCost.text = costs.food.ToString();
        woodCost.text = costs.wood.ToString();
        oreCost.text = costs.ore.ToString();

        if (myButton == null)
            myButton = GetComponent<Button>();
        myButton.interactable = GameManager.CanAfford(GameManager.LocalPlayerID, costs);
    }

    public void Button_ButtonPressed()
    {
        Debug.Log($"InteractionButton\t {gameObject} wants to build a {pawnData.PawnName}. \n", gameObject);
        InteractionMenuManager.Close();

        InputMessage newMessage = InputMessageGenerator.CreateBuildMessage(GameInputManager.SelectedPawn, 
                                                                           GameInputManager.SelectedHexCell, buttonPawn);
        InputMessageExecuter.Send(newMessage);
    }
}
