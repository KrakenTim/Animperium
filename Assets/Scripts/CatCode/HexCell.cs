
using UnityEngine;


public enum HexDirection
{
    NE, E, SE, SW, W, NW

}

public static class HexDirectionExtensions
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        return ((int)direction < 3) ? (direction + 3) : (direction - 3); //wenn direction < 3, direction + 3 wird verwendet sonst direction-3
    }
    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }
}

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;

    public Color color;

    #region Not in Tutorial

    private PlayerPawn pawnOnCell;
    public PlayerPawn Pawn => pawnOnCell;
    public bool HasPawn => pawnOnCell != null;

    #endregion Not in Tutorial

    [SerializeField]
    HexCell[] neighbors;

    [SerializeField]
    private bool Underground;

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    #region Not in Tutorial

    public void SetPawn(PlayerPawn pawn)
    {
        if (!HasPawn || pawn == null)
            pawnOnCell = pawn;
        else
            Debug.LogError($"Tried to set two Pawns onto same HexCell{coordinates.ToString()}!\n", this);
    }

    public bool IsNeighbor(HexCell cell)
    {
        foreach (var item in neighbors)
        {
            if (item == cell) return true;
        }

        return false; 
    }

    #endregion Not in Tutorial

}


