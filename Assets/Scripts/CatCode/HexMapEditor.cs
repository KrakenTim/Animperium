using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine;

public class HexMapEditor : MonoBehaviour
{
    public static HexMapEditor Instance;

    public static event System.Action<int> BrushSizeChanged;

    //public Color[] colors;

    public HexGrid hexGrid;

    //private Color activeColor;

    public int brushSize;

    #region Not in Tutorial

    public const int TERRAIN_Water = 3; // blue in Editor
    public const int TERRAIN_Earth = 4; // brown in Editor
    public const int TERRAIN_Rock = 5; // gray in Editor

    #endregion Not in Tutorial

    private int activeTerrainLevel;

    private int activeWaterLevel;

    int activeDecoLevel, activePlantLevel, activeSpecialIndex;

    private int terrainLevelIncrement;

    private int waterLevelIncrement;

    private bool applyFixedWaterLevel;
    private bool applyFixedTerrainLevel;

    bool applyWater;

    bool applyDecoLevel, applyPlantLevel, applySpecialIndex;

    int activeTerrainTypeIndex;

    //bool applyColor;

    void Awake()
    {
        //SelectColor(-1);
        SetTerrainTypeIndex(0);
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
        else if (Mouse.current.rightButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
        {
            Erase();
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

            if ((applyWater || waterLevelIncrement == 1) && cell.WaterLevel <= cell.Elevation)
            {
                cell.WaterLevel = Mathf.Max(0, cell.Elevation + 1);
            }
            else
            {
                cell.WaterLevel += waterLevelIncrement;
                cell.WaterLevel = Mathf.Max(0, cell.WaterLevel);
                if (cell.WaterLevel <= cell.Elevation && cell.Elevation > 0)
                {
                    cell.WaterLevel = 0;
                }
            }

            cell.Elevation += terrainLevelIncrement;

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

    public void SetTerrainTypeIndex(int index)
    {
        ResetEditSettings();
        activeTerrainTypeIndex = index;
    }

    public void SetPlantLevel(int plantLevel)
    {
        applyPlantLevel = true;
        activePlantLevel = plantLevel;
    }

    public void SetDecoLevel(int decoLevel)
    {
        applyDecoLevel = true;
        activeDecoLevel = decoLevel;
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
        if (BrushSizeChanged != null)
        {
            BrushSizeChanged.Invoke(brushSize);
        }
    }



    public void ApplyWater()
    {
        ResetEditSettings();
        applyWater = true;
    }

    public void SetTerrainElevation(int increment)
    {
        ResetEditSettings();
        terrainLevelIncrement = increment;
    }

    public void SetTerrainElevationOnFixedLevel(int elevationLevel)
    {
        ResetEditSettings();
        activeTerrainLevel = elevationLevel;
    }

    public void SetWaterElevation(int increment)
    {
        ResetEditSettings();
        waterLevelIncrement = increment;
    }
    public void SetWaterElevationOnFixedLevel(int elevationLevel)
    {
        ResetEditSettings();
        activeWaterLevel = elevationLevel;
    }

    public void Erase()
    {
        ResetEditSettings();
        applySpecialIndex = true;
        activeSpecialIndex = 0;
    }

    public void SetSpecialIndex(int index)
    {
        ResetEditSettings();
        applySpecialIndex = true;
        activeSpecialIndex = index;
    }

    private void ResetEditSettings()
    {
        applyDecoLevel = false;
        applyPlantLevel = false;
        applySpecialIndex = false;
        activeTerrainTypeIndex = -1;
        waterLevelIncrement = 0;
        terrainLevelIncrement = 0;
        applyWater = false;

        applyFixedWaterLevel = false;
        activeWaterLevel = 0;

        applyFixedTerrainLevel = false;
        activeTerrainLevel = 0;
    }
}