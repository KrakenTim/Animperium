using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Basic HUD which shows the player's resources and info about the currently selected or hovered unit.
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    private static PlayerHUD instance;

    [SerializeField] Image background;
    [SerializeField] TMPro.TMP_Text foodAmount;
    [SerializeField] TMPro.TMP_Text woodAmount;
    [SerializeField] TMPro.TMP_Text oreAmount;
    [Space]
    [SerializeField] GameObject endTurnButton;
 
    [Header("Pawn Info")]
    [SerializeField] GameObject pawnInfoRoot;
    [Space]
    [SerializeField] ColorableImage playerIcon;
    [SerializeField] ColorableImage pawnIcon;    
    [SerializeField] Image canActIcon;
    [Space]
    [SerializeField] TMPro.TMP_Text pawnType;
    [SerializeField] TMPro.TMP_Text pawnHP;
    [SerializeField] TMPro.TMP_Text pawnMP;

    PlayerPawn selectedPawn;

    private void Awake()
    {
        instance = this;

        GameManager.TurnStarted += UpdateHUD;
        GameInputManager.SelectPawn += UpdateSelectedPawn;

        FillValuesIn(null);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;

        GameManager.TurnStarted -= UpdateHUD;
        GameInputManager.SelectPawn -= UpdateSelectedPawn;
    }

    public static void UpdateHUD(int playerID)
    {
        instance.background.color = GameManager.GetPlayerColor(playerID);
        
        GameResources resources = GameManager.GetPlayerResources(GameManager.LocalPlayerID);
        instance.foodAmount.text = resources.food + " Food";
        instance.woodAmount.text = resources.wood + " Wood";
        instance.oreAmount.text = resources.ore + " Ore";

        instance.endTurnButton.SetActive(GameManager.InputAllowed);
    }

    private void UpdateSelectedPawn(PlayerPawn selectedPawn)
    {
        this.selectedPawn = selectedPawn;
        FillValuesIn(selectedPawn);
    }

    public static void UpdateShownPawn()
    {
        instance.FillValuesIn(instance.selectedPawn);
    }

    public static void HoverPawn(PlayerPawn hoveredPawn)
    {
        instance.FillValuesIn(hoveredPawn);
    }

    private void FillValuesIn(PlayerPawn selectedPawn)
    {
        if (selectedPawn == null)
        {
            pawnInfoRoot.SetActive(false);
            playerIcon.SetVisible(false);
            pawnIcon.SetVisible(false);
            canActIcon.enabled = false;
            return;
        }
        else
        {
            pawnInfoRoot.SetActive(true);
            playerIcon.SetVisible(true);
            pawnIcon.SetVisible(true);
        }

        playerIcon.SetPlayer(selectedPawn.PlayerID);
        pawnIcon.SetPawn(selectedPawn);

        canActIcon.enabled = selectedPawn.CanAct;
        pawnType.text = selectedPawn.FriendlyName;

        pawnHP.text = "HP " + selectedPawn.HP + "/" + selectedPawn.MaxHealth;
        if (selectedPawn.IsUnit)
        {
            pawnMP.text = "MP " + selectedPawn.MP + "/" + selectedPawn.MaxMovement;
            pawnMP.enabled = true;
        }
        else
            pawnMP.enabled = false;
    }

    public void Button_EndTurn()
    {
        if (!GameManager.InputAllowed) return;

        var message = InputMessageGenerator.CreateBasicMessage(ePlayeractionType.EndTurn);
        InputMessageExecuter.Send(message);
    }
}
