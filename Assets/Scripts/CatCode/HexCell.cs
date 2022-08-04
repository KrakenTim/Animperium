using UnityEngine;
using System.IO;

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
    int specialIndex;
    public int SpecialIndex
    {
        get
        {
            return specialIndex;
        }
        set
        {
            if (specialIndex != value)
            {
                specialIndex = value;
                RefreshSelfOnly();
            }
        }
    }
    public bool IsSpecial
    {
        get
        {
            return specialIndex > 0;
        }
    }
    public int TerrainTypeIndex
    {
        get
        {
            return terrainTypeIndex;
        }
        set
        {
            if (terrainTypeIndex != value)
            {
                terrainTypeIndex = value;
                Refresh();
            }
        }
    }

    public Color Color
    {
        get
        {
            return HexMetrics.colors[terrainTypeIndex];
        }
    }
    //    public Color color;
    int terrainTypeIndex;

    public int WaterLevel
    {
        get
        {
            return waterLevel;
        }
        set
        {
            if (waterLevel == value)
            {
                return;
            }
            waterLevel = value;
            Refresh();
        }
    }

    int waterLevel;
    public bool IsUnderwater
    {
        get
        {
            return waterLevel > elevation;
        }
    }

    #region NotInTutorial
    public bool IsDiggable
    {
        get
        {
            return terrainTypeIndex != HexMapEditor.TERRAIN_Rock && Elevation == HexGridManager.UNDIGGED_ELEVATION;
        }
    }

    public HexGridLayer gridLayer;

    public bool ShouldBeRockInUnderground
    {
        get
        {
            return IsUnderwater || terrainTypeIndex == HexMapEditor.TERRAIN_Rock;
        }
    }
    #endregion

    public int DecoLevel
    {
        get
        {
            return decoLevel;
        }
        set
        {
            if (decoLevel != value)
            {
                decoLevel = value;
                RefreshSelfOnly();
            }
        }
    }
    public int PlantLevel
    {
        get
        {
            return plantLevel;
        }
        set
        {
            if (plantLevel != value)
            {
                plantLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    int decoLevel, plantLevel;


    public HexCoordinates coordinates;

    int elevation;

    public RectTransform uiRect;

    public HexGridChunk chunk;
    public float WaterSurfaceY
    {
        get
        {
            return
                (waterLevel + HexMetrics.waterElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    #region Not in Tutorial

    private PlayerPawn pawnOnCell;
    public PlayerPawn Pawn => pawnOnCell;
    public bool HasPawn => pawnOnCell != null;

    private ResourceToken resource;
    public ResourceToken Resource => resource;
    public bool HasResource => resource != null;
    
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
            if (elevation == value)
            {
                return;
            }
            elevation = value;
            RefreshPosition();
            //Vector3 position = transform.localPosition;
            //position.y = value * HexMetrics.elevationStep;
            //position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
            //transform.localPosition = position;

            //Vector3 uiPosition = uiRect.localPosition;
            //uiPosition.z = -position.y;
            //uiRect.localPosition = uiPosition;
            //Refresh();

            if (HasPawn)
                Pawn.UpdatePosition();

            if (Resource != null)
                Resource.SetHexCell(this);

        }
    }

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }
    public Vector3 WorldPosition
    {
        get
        {
            return transform.position;
        }
    }

    [SerializeField] private Vector3 ObjectPositionOffsetCorrection;

    public Vector3 ObjectPosition
    {
        get
        {
            return transform.position + ObjectPositionOffsetCorrection;
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

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }
    void RefreshSelfOnly()
    {
        chunk.Refresh();
    }
    #region Not in Tutorial

    public void SetPawn(PlayerPawn pawn)
    {
        if (!HasPawn || pawn == null)
            pawnOnCell = pawn;
        else
            Debug.LogError($"Tried to set two Pawns onto same HexCell{coordinates.ToString()}!\n", this);
    }

    public void SetResource(ResourceToken newResource)
    {
        if (resource == null || newResource == null)
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

    /// <summary>
    /// sets visual aspects identical to the given Cell
    /// </summary>
    public void Copy(HexCell blueprint)
    {
        TerrainTypeIndex = blueprint.TerrainTypeIndex;
        Elevation = blueprint.Elevation;
        WaterLevel = blueprint.WaterLevel;

        DecoLevel = blueprint.DecoLevel;
        PlantLevel = blueprint.PlantLevel;
        SpecialIndex = blueprint.SpecialIndex;
    }

    /// <summary>
    /// True if a pawn might step onto the HexCell
    /// False if cell is under water or there's a cliff height difference between them.
    /// </summary>
    /// <param name="origin">the cell the pawn starts at</param>
    public bool CanMoveOnto(HexCell origin)
    {
        return Mathf.Abs(origin.Elevation - Elevation) < 2 && !IsUnderwater;
    }

    public int DistanceTo(HexCell other)
    {
        return coordinates.DistanceTo(other.coordinates);
    }

    #endregion Not in Tutorial

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)terrainTypeIndex);
        writer.Write((byte)elevation);
        writer.Write((byte)waterLevel);
        writer.Write((byte)decoLevel);
        writer.Write((byte)plantLevel);
        writer.Write((byte)specialIndex);
    }

    public void Load(BinaryReader reader)
    {
        terrainTypeIndex = reader.ReadByte();
        elevation = reader.ReadByte();
        RefreshPosition();
        waterLevel = reader.ReadByte();
        decoLevel = reader.ReadByte();
        plantLevel = reader.ReadByte();
        specialIndex = reader.ReadByte();
    }

    void RefreshPosition()
    {
        Vector3 position = transform.localPosition;
        position.y = elevation * HexMetrics.elevationStep;
        position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = -position.y;
        uiRect.localPosition = uiPosition;
        Refresh();
    }
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
