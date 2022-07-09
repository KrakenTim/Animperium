using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexGridLayer
{
    Surface = 0,
    Underground = 1
}

public class HexGridManager : MonoBehaviour
{
    public const int UNDIGGED_ELEVATION = 2;
    public const int DIGGED_ELEVATION = 0;

    public static HexGridManager Current => GameManager.Instance.hexGridManager;

    [SerializeField] HexGrid surface;
    public HexGrid Surface => surface;
    [SerializeField] HexGrid underground;
    public HexGrid Underground => underground;

    const int HIGHEST_LAYER = 1;

    [Header("Resource Generation")]
    [SerializeField] int oresPer100Cells = 10;
    [SerializeField] ResourceToken orePrefab;

    private void Awake()
    {
        CreateUnderground();

        SetHexCellGridLayers();

        DistributeOres();
    }

    /// <summary>
    /// Returns the HexCell, considers if its on the surface or in the underground
    /// </summary>
    public HexCell GetHexCell(Vector3 worldposition)
    {
        if (surface.WorldArea().InArea(worldposition))
            return surface.GetHexCell(worldposition);
        else
            return underground.GetHexCell(worldposition);
    }

    public static bool IsWalkable(HexCell cell)
    {
        if (cell.HasPawn || cell.Resource != null) return false;

        if (IsSurface(cell)) return true;


        return cell.Elevation == DIGGED_ELEVATION;
    }

    public HexCell GetHexCell(HexCoordinates coordinates, int layer)
    {
        if (layer > 0)
            return underground.GetCell(coordinates);
        else
            return surface.GetCell(coordinates);
    }

    /// <summary>
    /// HexCells of all layers for given coordinate
    /// </summary>
    public HexCell[] GetHexCells(HexCoordinates coordinates)
    {
        HexCell[] result = new HexCell[HIGHEST_LAYER + 1];

        for (int i = 0; i < result.Length; i++)
            result[i] = GetHexCell(coordinates, i);

        return result;
    }
    
    public HexGrid GetGrid(HexCell cell)
    {
        if (cell.gridLayer == HexGridLayer.Surface)
            return Surface;
        else
            return Underground;
    }

    public static bool IsSurface(HexCell cell)
    {
        return cell.gridLayer == HexGridLayer.Surface;
    }

    public static bool IsUnderground(HexCell cell)
    {
        return cell.gridLayer == HexGridLayer.Underground;
    }

    private void SetHexCellGridLayers()
    {
        foreach (var cell in Surface.GetAllCells())
            cell.gridLayer = HexGridLayer.Surface;

        foreach (var cell in Underground.GetAllCells())
            cell.gridLayer = HexGridLayer.Underground;
    }

    private void CreateUnderground()
    {
        HexCell[] surfaceCells = surface.GetAllCells();

        underground.CreateMap(surface.cellCountX, surface.cellCountZ);
        HexCell[] undergroundCells = underground.GetAllCells();

        if (surfaceCells.Length != undergroundCells.Length)
        {
            Debug.LogError($"Number of Cells in Surface({surfaceCells.Length}) and Underground({undergroundCells.Length}) dont match!\n", this);
            return;
        }

        for (int cellID = 0; cellID < undergroundCells.Length; cellID++)
        {
            undergroundCells[cellID].Elevation = UNDIGGED_ELEVATION;

            undergroundCells[cellID].TerrainTypeIndex = surfaceCells[cellID].ShouldBeRockInUnderground ? HexMapEditor.TERRAIN_Rock : HexMapEditor.TERRAIN_Earth;
        }

        foreach (var chunk in underground.GetAllChunks())
            chunk.Refresh();
    }

    private void DistributeOres()
    {
        Random.InitState(GameManager.RandomGenerationKey);

        HexCell[] undergroundCells = underground.GetAllCells();

        int halfLenght = undergroundCells.Length / 2;

        HashSet<int> oreSpots = new HashSet<int>();

        int oresInMap = (undergroundCells.Length * oresPer100Cells)/200;

        while (oreSpots.Count < oresInMap)
        {
            int pos = Random.Range(0, halfLenght + 1);
            if (!oreSpots.Contains(pos) && undergroundCells[pos].IsDiggable && undergroundCells[undergroundCells.Length - (1 + pos)].IsDiggable)
                oreSpots.Add(pos);
        }

        foreach (var spot in oreSpots)
        {
            Instantiate(orePrefab, undergroundCells[spot].transform.position, Quaternion.identity, GameManager.Instance.transform.GetChild(0));
            Instantiate(orePrefab, undergroundCells[undergroundCells.Length - (1 + spot)].transform.position, Quaternion.identity, GameManager.Instance.transform.GetChild(0));
        }
    }

    public HexCell OtherLayerCell(HexCell cell)
    {
        if (IsSurface(cell))
            return GetHexCell(cell.coordinates, 1);
        else
            return GetHexCell(cell.coordinates, 0);
    }

    public void DigAwayCircle(HexCell cell)
    {
        if (!IsUnderground(cell))
            cell = GetHexCell(cell.coordinates, 1);

        HashSet<HexCell> diggedAwayCells = underground.GetNeighbours(cell, 1, withCenter: true);

        foreach (var hexCell in diggedAwayCells)
        {
            DigAwayCell(hexCell);
        }
    }

    public bool DigAwayCell(HexCell hexCell)
    {
        if (!hexCell.IsDiggable) return false;

        hexCell.Elevation = DIGGED_ELEVATION;
        return true;
    }
}
