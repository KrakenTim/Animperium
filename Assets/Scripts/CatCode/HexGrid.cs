using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class HexGrid : MonoBehaviour
{
    int chunkCountX, chunkCountZ;

    #region Not in Tutorial
    [SerializeField] bool underground;

    public int ChunkCountX => chunkCountX;
    public int ChunkCountZ => chunkCountZ;
    #endregion Not in Tutorial

    public int cellCountX = 20, cellCountZ = 15;
    public int seed;

    public Color[] colors;
    //public Color defaultColor = Color.white;
    public Color touchedColor = Color.green;
    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public Texture2D noiseSource;
    public HexGridChunk chunkPrefab;
    HexCell[] cells;
    HexGridChunk[] chunks;
    //	Canvas gridCanvas;
    //	HexMesh hexMesh;

    public void Awake()
    {
        #region Not in Tutorial
        if (underground) return;
        TempMapSaves ts = GetComponent<TempMapSaves>();
        if (ts && ts.LoadsInsteadOfHexGrid) return;
        #endregion Not in Tutorial

        HexMetrics.noiseSource = noiseSource;
        HexMetrics.InitializeHashGrid(seed);
        HexMetrics.colors = colors;
        //		gridCanvas = GetComponentInChildren<Canvas>();
        //		hexMesh = GetComponentInChildren<HexMesh>();
        CreateMap(cellCountX, cellCountZ);
    }

    public bool CreateMap(int x, int z)
    {
        if (x <= 0 || x % HexMetrics.chunkSizeX != 0 || z <= 0 || z % HexMetrics.chunkSizeZ != 0)
        {
            Debug.LogError("Unsupported map size.");
            return false;
        }
        if (chunks != null)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                Destroy(chunks[i].gameObject);
            }
        }
        cellCountX = x;
        cellCountZ = z;
        chunkCountX = cellCountX / HexMetrics.chunkSizeX;
        chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();

        return true;
    }

    //	void Start () {
    //		hexMesh.Triangulate(cells);
    //	}

    void OnEnable()
    {
        if (!HexMetrics.noiseSource)
        {
            HexMetrics.noiseSource = noiseSource;
            HexMetrics.InitializeHashGrid(seed);
            HexMetrics.colors = colors;
        }
    }

    //	public void Refresh () {
    //		hexMesh.Triangulate(cells);
    //	}

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = x * 10f;
        position.y = 0f;
        position.z = z * 10f;

        position.x = x * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        //cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        //cell.color = defaultColor;


        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;

        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }
    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;

        if (index >= 0 && index < cells.Length)
            return cells[index];
        return
            null;
    }
    /*
    public void ColorCell(Vector3 position, Color color)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexCell cell = cells[index];
        cell.color = color;
        hexMesh.Triangulate(cells);
        Debug.Log("touched at " + coordinates.ToString());
    }*/

    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x;
        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;
        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);
        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }


        return new HexCoordinates(iX, iZ);
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
                chunk.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void ShowUI(bool visible)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].ShowUI(visible);
        }
    }

    #region Not in Tutorial

    public HexCell GetHexCell(Vector3 worldposition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldposition);
        HexCoordinates coordinates = HexCoordinates.FromPosition(localPosition);
        return GetHexCell(coordinates);
    }

    public HexCell GetHexCell(HexCoordinates coordinates)
    {
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
        {
            return null;
        }
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
        {
            return null;
        }
        return cells[x + z * cellCountX];
    }

    public HexCell[] GetAllCells()
    {
        return cells;
    }

    public HexGridChunk[] GetAllChunks()
    {
        return chunks;
    }

    public void Clear()
    {
        if (chunks == null) return;

        for (int i = 0; i < chunks.Length; i++)
        {
            if (chunks[i])
                Destroy(chunks[i].gameObject);
        }
        chunks = null;
    }

    public HashSet<HexCell> GetNeighbours(HexCell center, int size, bool withCenter = false)
    {
        HashSet<HexCell> neighbourCells = new HashSet<HexCell>();

        if (size < 1)
        {
            if (withCenter)
                neighbourCells.Add(center);

            return neighbourCells;
        }

        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - size; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + size; x++)
            {
                neighbourCells.Add(GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + size; z > centerZ; z--, r++)
        {
            for (int x = centerX - size; x <= centerX + r; x++)
            {
                neighbourCells.Add(GetCell(new HexCoordinates(x, z)));
            }
        }

        if (!withCenter)
            neighbourCells.Remove(center);

        neighbourCells.Remove(null);

        return neighbourCells;
    }

    /// <summary>
    /// x,y is horizontal, z,w is vertical extension.
    /// </summary>
    public Vector4 WorldArea()
    {
        Vector4 area = new Vector4();
        area.x = cells[0].transform.position.x;
        area.z = cells[0].transform.position.z;

        area.y = cells[cells.Length - 1].transform.position.x;
        area.w = cells[cells.Length - 1].transform.position.z;

        return area;
    }

    #endregion Not in Tutorial


    public void Save(BinaryWriter writer)
    {
        writer.Write(cellCountX);
        writer.Write(cellCountZ);
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Save(writer);
        }
    }

    public void Load(BinaryReader reader, int header)
    {
        int x = 20, z = 15;
        if (header >= 1)
        {
            x = reader.ReadInt32();
            z = reader.ReadInt32();
        }
        if (x != cellCountX || z != cellCountZ || cells == null)
        {
            if (!CreateMap(x, z))
            {
                return;
            }
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Load(reader);
        }
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].Refresh();
        }
    }
}
