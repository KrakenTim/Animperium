using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A variant of a regular player pawn that can't be attacked, hovered over or clicked.
/// Use: Blocks the field below, so that player pawn can't move onto it.
/// </summary>
public class BlockedPawn : PlayerPawn
{
    public override bool IsActivePlayerPawn => false;
    
    public override bool IsEnemyOf(int otherPlayerID) => false;
    public override bool IsEnemyOf(PlayerPawn otherPawn) => false;

    protected override void Awake()
    {
        base.Awake();

        int rotationFactor = HexCoordinates.X * HexCoordinates.Y * HexCoordinates.Z + Mathf.RoundToInt(transform.position.x);
        
        Vector3 angle = transform.eulerAngles;
        angle.y = (rotationFactor % 6) * 60f;

        transform.eulerAngles = angle;
    }

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

#if UNITY_EDITOR
/// <summary>
/// Extends the default Unity Editor for the Class.
/// </summary>
[CustomEditor(typeof(BlockedPawn))]
public class BlockedPawnEditor : PlayerPawnEditor
{
    //identical to base class editor
}
#endif // UNITY_EDITOR