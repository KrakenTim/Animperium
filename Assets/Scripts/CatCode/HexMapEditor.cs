using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine;

public class HexMapEditor : MonoBehaviour
{
    public static HexMapEditor Instance;

    public static event System.Action<int> BrushSizeChanged;

    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    public int brushSize;
    
    #region Not in Tutorial
    public int tempSaveColorID;
    #endregion Not in Tutorial

    private int activeElevation;

    int activeWaterLevel;
    
    int activeUrbanLevel;

    bool applyElevation;

    bool applyWaterLevel = true;

    bool applyUrbanLevel;

    bool applyColor;


    void Awake()
    {
        SelectColor(-1);
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
    public void SetApplyUrbanLevel(bool toggle)
    {
        applyUrbanLevel = toggle;
    }

    public void SetUrbanLevel(float level)
    {
        activeUrbanLevel = (int)level;
    }

    void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (applyColor)
            {
                cell.Color = activeColor;

                # region Not in Tutorial
                cell.tempSaveColorID = tempSaveColorID;
                #endregion Not in Tutorial
            }
            if (applyElevation)
            {
                cell.Elevation = activeElevation;
            }
            if (applyWaterLevel)
            {
                cell.WaterLevel = activeWaterLevel;
            }
            if (applyUrbanLevel)
            {
                cell.UrbanLevel = activeUrbanLevel;
            }
            //		hexGrid.Refresh();
        }
    }

    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor)
        {
            activeColor = colors[index];

            #region Not in Tutorial
            tempSaveColorID = index;
            #endregion Not in Tutorial
        }
    }

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
        if(BrushSizeChanged != null)
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