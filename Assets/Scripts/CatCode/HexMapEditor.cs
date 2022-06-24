using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine;
using System.IO;

public class HexMapEditor : MonoBehaviour
{
    public static HexMapEditor Instance;

    public static event System.Action<int> BrushSizeChanged;

    //public Color[] colors;

    public HexGrid hexGrid;

    //private Color activeColor;

    public int brushSize;

    #region Not in Tutorial

    public const int COLOR_Water = 3; // blue in Editor
    public int tempSaveColorID;

    #endregion Not in Tutorial

    private int activeElevation;

    int activeWaterLevel;

    int activeDecoLevel, activePlantLevel, activeSpecialIndex;

    bool applyElevation;

    bool applyWaterLevel;

    bool applyDecoLevel, applyPlantLevel, applySpecialIndex;

    int activeTerrainTypeIndex;

    //bool applyColor;


    void Awake()
    {
        //SelectColor(-1);
        SetTerrainTypeIndex(-1);
        applyElevation = false;
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) { Instance = null; }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCells(hexGrid.GetCell(hit.point));
        }
    }
    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }
    public void SetApplyDecoLevel(bool toggle)
    {
        applyDecoLevel = toggle;
    }

    public void SetDecoLevel(float level)
    {
        activeDecoLevel = (int)level;
    }
    public void SetApplyPlantLevel(bool toggle)
    {
        applyPlantLevel = toggle;
    }

    public void SetPlantLevel(float level)
    {
        activePlantLevel = (int)level;
    }
    public void SetApplySpecialIndex(bool toggle)
    {
        applySpecialIndex = toggle;
    }

    public void SetSpecialIndex(float index)
    {
        activeSpecialIndex = (int)index;
    }
    public void SetTerrainTypeIndex(int index)
    {
        activeTerrainTypeIndex = index;
    }
    void EditCell(HexCell cell)
    {
        if (cell)
        {
            /*if (applyColor)
            {
                cell.Color = activeColor;

                # region Not in Tutorial
                cell.tempSaveColorID = tempSaveColorID;
                #endregion Not in Tutorial
            }*/
            if (activeTerrainTypeIndex >= 0)
            {
                cell.TerrainTypeIndex = activeTerrainTypeIndex;
            }
            if (applyElevation)
            {
                cell.Elevation = activeElevation;
            }
            if (applyWaterLevel)
            {
                cell.WaterLevel = activeWaterLevel;
            }
            if (applySpecialIndex)
            {
                cell.SpecialIndex = activeSpecialIndex;
            }
            if (applyDecoLevel)
            {
                cell.DecoLevel = activeDecoLevel;
            }
            //		hexGrid.Refresh();
            if (applyPlantLevel)
            {
                cell.PlantLevel = activePlantLevel;
            }

        }
    }

    // /*    public void SelectColor(int index)
    //    {
    //        applyColor = index >= 0;
    //        if (applyColor)
    //        {
    //            activeColor = colors[index];

    //            #region Not in Tutorial
    //            tempSaveColorID = index;
    //            #endregion Not in Tutorial
    //        }
    //    }*/

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }
    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
        if (BrushSizeChanged != null)
        {
            BrushSizeChanged.Invoke(brushSize);
        }
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }
    public void SetApplyWaterLevel(bool toggle)
    {
        applyWaterLevel = toggle;
    }

    public void SetWaterLevel(float level)
    {
        activeWaterLevel = (int)level;
    }

    

}