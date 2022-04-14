using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private static PlayerHUD instance;

    [SerializeField] Image background;
    [SerializeField] TMPro.TMP_Text foodAmount;
    [Header("Pawn Info")]
    [SerializeField] GameObject pawnInfoRoot;
    [SerializeField] Image playerIcon;
    [SerializeField] Image canActIcon;
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

    // Start is called before the first frame update
    void Start()
    {
        UpdateHUD(GameManager.CurrentPlayerID);
    }
    private void UpdateHUD(int playerID)
    {
        background.color = GameManager.GetPlayerColor(playerID);

        foodAmount.text = GameManager.GetPlayerFood(playerID) + " Food";
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
            canActIcon.enabled = false;
            return;
        }
        else
        {
            pawnInfoRoot.SetActive(true);
            playerIcon.enabled = true;
        }

        playerIcon.sprite = selectedPawn.PlayerIcon;
        canActIcon.enabled = selectedPawn.CanAct;
        pawnType.text = selectedPawn.PawnType.ToString();

        pawnHP.text = "HP " + selectedPawn.HP + "/" + selectedPawn.MaxHealth;
        pawnMP.text = "MP " + selectedPawn.MP + "/" + selectedPawn.MaxMovement;
        pawnMP.enabled = selectedPawn.IsUnit;
    }   

    public void Button_EndTurn()
    {
        GameManager.EndTurn();
    }

}
