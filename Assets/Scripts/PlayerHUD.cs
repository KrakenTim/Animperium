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
    [Header("Pawn Info")]
    [SerializeField] GameObject pawnInfoRoot;
    [SerializeField] Image playerIcon;
    [SerializeField] Image pawnIcon;
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
        GameInputManager.SelectedPawn += UpdateSelectedPawn;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;

        GameManager.TurnStarted -= UpdateHUD;
        GameInputManager.SelectedPawn -= UpdateSelectedPawn;
    }

    public static void UpdateHUD(int playerID)
    {
        instance.background.color = GameManager.GetPlayerColor(playerID);

        instance.foodAmount.text = GameManager.GetPlayerResources(playerID).food + " Food";
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
            playerIcon.enabled = false;
            pawnIcon.enabled = false;
            canActIcon.enabled = false;
            return;
        }
        else
        {
            pawnInfoRoot.SetActive(true);
            playerIcon.enabled = true;
            pawnIcon.enabled = true;
        }

        playerIcon.sprite = selectedPawn.PlayerIcon;
        pawnIcon.sprite = selectedPawn.PawnIcon;
        canActIcon.enabled = selectedPawn.CanAct;
        pawnType.text = selectedPawn.PawnType.ToString();

        pawnHP.text = "HP " + selectedPawn.HP + "/" + selectedPawn.MaxHealth;
        pawnMP.text = "MP " + selectedPawn.MP + "/" + selectedPawn.MaxMovement;
        pawnMP.enabled = selectedPawn.IsUnit;
    }

    public void Button_EndTurn()
    {
        if (!GameManager.InputAllowed) return;

        var message = InputMessageGenerator.CreateBasicMessage(ePlayeractionType.EndTurn);
        InputMessageExecuter.Send(message);
    }
}
