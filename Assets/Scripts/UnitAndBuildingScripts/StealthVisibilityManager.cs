using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Visibility of stealth units of a certain player.
/// </summary>
public class StealthVisibilityManager : MonoBehaviour
{
    const int detectionDistance = 3;

    private PlayerValues MyPlayerValues;

    private List<PlayerPawn> stealthPawns = new List<PlayerPawn>();

    private void OnEnable()
    {
        PlayerPawn.OnPawnMoved += UpdateVisibility;
        GameManager.LocalPlayerChanged += UpdateAllVisibilities;
    }

    private void OnDisable()
    {
        PlayerPawn.OnPawnMoved -= UpdateVisibility;
        GameManager.LocalPlayerChanged -= UpdateAllVisibilities;
    }

    public void Initialized(PlayerValues newPlayerValues)
    {
        MyPlayerValues = newPlayerValues;

        stealthPawns.Clear();
        foreach (var pawn in MyPlayerValues.ownedPawns)
        {
            if (pawn.CanStealth)
                stealthPawns.Add(pawn);
        }
    }

    /// <summary>
    /// Checks if update of own pawns stealth status is needed and calls the cirresponding functions. 
    /// </summary>
    private void UpdateVisibility(PlayerPawn movedPawn, HexCell oldCell, HexCell newCell)
    {
        // Isn't own pawn

        if (movedPawn.PlayerID != MyPlayerValues.playerID)
        {
            if (movedPawn.IsEnemyOf(MyPlayerValues.playerID))
                MovedEnemyPawn(movedPawn, oldCell, newCell);

            // Do nothing if allied pawn was moved
        }
        else // Own pawn
        {
            if (movedPawn.CanStealth)
                MovedOwnStealthPawn(movedPawn, oldCell, newCell);

            // Do nothing if own non-stealth pawn was moved
        }
    }

    /// <summary>
    /// Checks if movement of enemy pawn affects stealth statusof own pawns. And updates if needed.
    /// </summary>
    private void MovedEnemyPawn(PlayerPawn enemyPawn, HexCell oldCell, HexCell newCell)
    {
       // Debug.Log($"Enemy moved: {enemyPawn.gameObject.name} ({enemyPawn.PawnType}) from {oldCell?.coordinates} to {newCell?.coordinates}\n", enemyPawn);

        HashSet<PlayerPawn> alreadyHandled = new HashSet<PlayerPawn>();

        if (newCell != null)
        {
            foreach (var pawn in stealthPawns)
            {
                if (pawn.HexCell.DistanceTo(newCell) <= detectionDistance)
                {
                    pawn.SetStealthed(false);
                    alreadyHandled.Add(pawn);
                }
            }
        }

        if (oldCell != null)
        {
            foreach (var pawn in stealthPawns)
            {
                // If visible because of new cell, we don't need to check further.
                if (alreadyHandled.Contains(pawn)) continue;

                // Check if pawn was seen before and is now seen by some other enemy. 
                if (pawn.HexCell.DistanceTo(oldCell) <= detectionDistance)
                    UpdatePawnVisibility(pawn);
            }
        }
    }

    /// <summary>
    /// Add or removed given pawn from stealth pawn list if needed, updates the pawns stealth status.
    /// </summary>
    private void MovedOwnStealthPawn(PlayerPawn stealthPawn, HexCell oldCell, HexCell newCell)
    {
       // Debug.Log($"SteahlthPawn moved: {stealthPawn.gameObject.name} ({stealthPawn.PawnType}) from {oldCell?.coordinates} to {newCell?.coordinates}\n", stealthPawn);

        // Add newly created pawn
        if (oldCell == null && !stealthPawns.Contains(stealthPawn))
            stealthPawns.Add(stealthPawn);

        //removed destroyed pawn
        if (newCell == null)
        {
            stealthPawns.Remove(stealthPawn);
            return;
        }

        UpdatePawnVisibility(stealthPawn);
    }

    /// <summary>
    /// Check if is seen by enemy pawn on neighbouring hexcells, updates steath of pawn according. 
    /// </summary>
    private void UpdatePawnVisibility(PlayerPawn stealthPawn)
    {
        HashSet<HexCell> neighbourCells = GameManager.HexGrid.GetNeighbours(stealthPawn.HexCell, detectionDistance);

        bool isSeen = false;
        foreach (var cell in neighbourCells)
        {
            if (cell.HasPawn && cell.Pawn.IsEnemyOf(MyPlayerValues.playerID))
            {
                isSeen = true;
                break;
            }
        }
        stealthPawn.SetStealthed(!isSeen);
    }

    /// <summary>
    /// All stealth units get their visibility updated.
    /// </summary>
    private void UpdateAllVisibilities()
    {
        foreach (var pawn in stealthPawns)
        {
            pawn.UpdateVisuals();
        }
    }
}
