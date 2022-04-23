using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
