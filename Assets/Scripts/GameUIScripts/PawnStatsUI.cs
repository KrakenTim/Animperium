using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PawnStatsUI : MonoBehaviour
{
    [SerializeField] ColorableImage icon;
    [SerializeField] LocalisedText pawnName;
    [SerializeField] Image healthBar;
    [Space]
    [SerializeField] TMP_Text healthValue;
    [SerializeField] TMP_Text movementValue;
    [SerializeField] TMP_Text attackValue;
    [SerializeField] GameObject canAct;
    [Space]
    [SerializeField] Image coloredBackground;
    [SerializeField, Range(0f, 1f)] float tintIntensity = 0.25f;

    private PlayerPawn myPawn;
    public PlayerPawn Pawn => myPawn;
    private Color originalColor;

    private void Awake()
    {
        originalColor = coloredBackground.color;
    }

    private void OnDestroy()
    {
        SetPawn(null);
    }

    /// <summary>
    /// Sets the current pawn and connects with it's value change action.
    /// </summary>
    public void SetPawn(PlayerPawn newPawn)
    {
        if (myPawn != null)
            myPawn.OnValueChange -= UpdateChangingValues;

        myPawn = newPawn;

        if (myPawn != null)
            myPawn.OnValueChange += UpdateChangingValues;

        UpdateUnchangingValues();
        UpdateChangingValues();

        SetVisible(myPawn != null);
    }

    private void UpdateUnchangingValues()
    {
        if (myPawn == null) return;

        icon.SetPawn(myPawn);

        pawnName.Set(AnimperiumLocalisation.GetIdentifier(myPawn.PawnType));

        attackValue.text = myPawn.AttackPower.ToString();

        if (originalColor.a == 0)
            originalColor = coloredBackground.color;

        coloredBackground.color = Color.Lerp(originalColor, GameManager.GetPlayerColor(myPawn.PlayerID), tintIntensity);
    }

    /// <summary>
    /// Fill the fields and elements with the values of the currently used pawn.
    /// </summary>
    private void UpdateChangingValues()
    {
        if (myPawn == null) return;

        healthBar.fillAmount = myPawn.HP / (float)myPawn.MaxHealth;
        healthValue.text = myPawn.HP + "/" + myPawn.MaxHealth;

        movementValue.text = myPawn.MP + "/" + myPawn.MaxMovement;

        canAct.SetActive(myPawn.CanAct);
    }

    /// <summary>
    /// Make the element (in)visible.
    /// </summary>
    public void SetVisible(bool shouldBeVisible)
    {
        gameObject.SetActive(shouldBeVisible);
    }
}
