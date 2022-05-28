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

    [Header("Pawn Info")]
    [SerializeField] GameObject pawnInfoRoot;
    [SerializeField] GameObject pawnInfoAttackPower;
    [Space]
    [SerializeField] ColorableImage playerIcon;
    [SerializeField] ColorableImage pawnIcon;
    [SerializeField] Image canActIcon;
    [Space]
    [SerializeField] LocalisedText pawnType;
    [SerializeField] TMPro.TMP_Text pawnHP;
    [SerializeField] TMPro.TMP_Text pawnMP;
    [SerializeField] TMPro.TMP_Text attackPower;

    PlayerPawn selectedPawn;
    public PlayerPawn SelectedPawn => instance.selectedPawn;

    bool attachedToPawn = false;

    private void Awake()
    {
        instance = this;

        GameInputManager.SelectPawn += UpdateSelectedPawn;

        FillValuesIn(null);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;

        GameInputManager.SelectPawn -= UpdateSelectedPawn;

        SetSelectedPawn(null);
    }

    private void UpdateSelectedPawn(PlayerPawn selectedPawn)
    {
        SetSelectedPawn(selectedPawn);
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
            pawnInfoAttackPower.SetActive(false);
            playerIcon.SetVisible(false);
            pawnIcon.SetVisible(false);
            canActIcon.enabled = false;
            return;
        }
        else
        {
            pawnInfoRoot.SetActive(true);
            pawnInfoAttackPower.SetActive(true);
            playerIcon.SetVisible(true);
            pawnIcon.SetVisible(true);
        }

        playerIcon.SetPlayer(selectedPawn.PlayerID);
        pawnIcon.SetPawn(selectedPawn);

        canActIcon.enabled = selectedPawn.CanAct;
        pawnType.Set(AnimperiumLocalisation.GetIdentifier(selectedPawn.PawnType));

        pawnHP.text = "HP " + selectedPawn.HP + "/" + selectedPawn.MaxHealth;

        if (selectedPawn.IsUnit)
        {
            pawnMP.text = "MP " + selectedPawn.MP + "/" + selectedPawn.MaxMovement;
            pawnMP.enabled = true;
        }
        else
        {
            pawnMP.enabled = false;
        }

        if (selectedPawn.AttackPower > 0)
        {
            attackPower.text = Localisation.Instance.Get(AnimperiumLocalisation.ID_AttackPower) + " " + selectedPawn.AttackPower;
            pawnInfoAttackPower.SetActive(true);
        }
        else
            pawnInfoAttackPower.SetActive(false);
    }

    /// <summary>
    /// Detaches itself from old pawn, sets new pawn and attaches itself to it for updates on value changes.
    /// </summary>
    private void SetSelectedPawn(PlayerPawn newPawn)
    {
        if (selectedPawn != null)
            selectedPawn.OnValueChange -= UpdateShownPawn;

        selectedPawn = newPawn;

        if (selectedPawn != null)
            selectedPawn.OnValueChange += UpdateShownPawn;

        FillValuesIn(selectedPawn);
    }
}
