using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridManager : MonoBehaviour
{
    public static HexGridManager Current => GameManager.Instance.hexGridManager;

    [SerializeField] HexGrid surface;
    [SerializeField] HexGrid underground;
    [Space]
    [SerializeField] HexMapEditor editor;

    [SerializeField] int diggableID = 4;
    [SerializeField] int rockID = 5;

    private void Awake()
    {
        CreateUnderground();
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

    public HexCell GetHexCell(HexCoordinates coordinates, int layer)
    {
        if (layer > 0)
            return underground.GetCell(coordinates);
        else
            return surface.GetCell(coordinates);
    }

    public int GetHexCellLayer(HexCell cell)
    {
        Debug.Log($"Underground: {underground.WorldArea()} {underground.WorldArea().InArea(cell.transform.position)} " +
                  $"Surface: {surface.WorldArea()} {underground.WorldArea().InArea(cell.transform.position)} Position: {cell.transform.position}\n");

        if (underground.WorldArea().InArea(cell.transform.position))
            return 1;
        else
            return 0;
    }

    public HexGrid GetGrid(HexCell cell)
    {
        if (GetHexCellLayer(cell) == 0)
            return surface;
        else
            return underground;
    }

    private void CreateUnderground()
    {
        HexCell[] surfaceCells = surface.GetAllCells();
        HexCell[] undergroundCells = underground.GetAllCells();

        if (surfaceCells.Length != undergroundCells.Length)
        {
            Debug.LogError($"Number of Cells in Surface({surfaceCells.Length}) and Underground({undergroundCells.Length}) dont match!\n", this);
            return;
        }

        int cellColorID;
        for (int cellID = 0; cellID < undergroundCells.Length; cellID++)
        {
            //undergroundCells[cellID].Elevation = int.Parse(nextLine[0]);

            if (editor)
            {
                cellColorID = surfaceCells[cellID].IsTunnelPossible ? diggableID : rockID;

                undergroundCells[cellID].tempSaveColorID = cellColorID;
                undergroundCells[cellID].Color = editor.colors[cellColorID];
            }
        }

        foreach (var chunk in underground.GetAllChunks())
            chunk.Refresh();
    }
}
