using UnityEngine;


public enum HexDirection
{
    NE, E, SE, SW, W, NW

}
public enum HexEdgeType
{
    Flat, Slope, Cliff
}


public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;

    public Color color;

    int elevation;

    public RectTransform uiRect;

    #region Not in Tutorial

    private PlayerPawn pawnOnCell;
    public PlayerPawn Pawn => pawnOnCell;
    public bool HasPawn => pawnOnCell != null;

    private RessourceToken resource;
    public RessourceToken Resource => resource;

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
    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;
        }
    }
    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }
    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
    }
    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }

    #region Not in Tutorial

    public void SetPawn(PlayerPawn pawn)
    {
        if (!HasPawn || pawn == null)
            pawnOnCell = pawn;
        else
            Debug.LogError($"Tried to set two Pawns onto same HexCell{coordinates.ToString()}!\n", this);
    }

    public void SetResource(RessourceToken newResource)
    {
        if (resource == null || newResource ==null)
            resource = newResource;
        else
            Debug.LogError($"Resource to set two Pawns onto same HexCell{coordinates.ToString()}!\n", this);
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