using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic HUD which shows the player's resources and info about the currently selected or hovered unit.
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    private static PlayerHUD instance;

    [SerializeField] PawnStatsUI pawnTooltip;
    [SerializeField] PawnStatsUI selectedPawnStats;

    PlayerPawn HoveredPawn => pawnTooltip.Pawn;
    PlayerPawn SelectedPawn => selectedPawnStats.Pawn;

    private void Awake()
    {
        instance = this;

        GameInputManager.SelectPawn += SetSelectedPawn;

        SetSelectedPawn(null);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;

        GameInputManager.SelectPawn -= SetSelectedPawn;
    }

    public static void HoverPawn(PlayerPawn hoveredPawn)
    {
        instance.pawnTooltip.SetPawn(hoveredPawn);
    }

    public static void UnHoverPawn()
    {
        instance.pawnTooltip.SetPawn(null);
    }

    /// <summary>
    /// Detaches itself from old pawn, sets new pawn and attaches itself to it for updates on value changes.
    /// </summary>
    private void SetSelectedPawn(PlayerPawn newPawn)
    {
        selectedPawnStats.SetPawn(newPawn);
    }

    /// <summary>
    /// Checks if shown pawns still exists. Selected is handled by GameInputManager.
    /// </summary>
    public static void UpdateExistingPawns()
    {
       if (instance.HoveredPawn?.HexCell == null)
            UnHoverPawn();
    }
}