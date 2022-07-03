using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TunnelEntryPawn : PlayerPawn
{
    // TODO(23.06.22) LookAway

    public override void Damaged(PlayerPawn attacker, int damageAmount)
    {
        foreach (var item in HexGridManager.Current.GetHexCells(HexCoordinates))
        {
            if (!item.HasPawn || item.Pawn.PawnType != PawnType)
            {
                Debug.LogError($"Tunnel Entry({gameObject.name}) at {transform.position} has no partner!\n", this);
                continue;
            }
            
            ((TunnelEntryPawn)item.Pawn).ActualDamaged(attacker, damageAmount);
        }
    }

    private void ActualDamaged(PlayerPawn attacker, int damageAmount) => base.Damaged(attacker, damageAmount);
}

#if UNITY_EDITOR
/// <summary>
/// Extends the default Unity Editor for the Class.
/// </summary>
[CustomEditor(typeof(TunnelEntryPawn))]
public class TunnelEntryPawnEditor : PlayerPawnEditor
{
}
#endif // UNITY_EDITOR