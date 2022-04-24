using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A variant of a regular player pawn that can't be attacked, hovered over or clicked.
/// Use: Blocks the field below, so that player pawn can't move onto it.
/// </summary>
public class BlockedPawn : PlayerPawn
{
    public override bool IsPlayerPawn => false;

    public override bool IsEnemy => false;

    public override void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
