using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{
    HexCell[] cells;

    public HexMesh terrain;
    //	HexMesh hexMesh;    
    Canvas gridCanvas;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
//		hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
        ShowUI(false);
    }

//	void Start () {
//		hexMesh.Triangulate(cells);
//	}

    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
    }
    public void Refresh()
    {
        //		hexMesh.Triangulate(cells);
        enabled = true;
    }

    public void ShowUI(bool visible)
    {
        gridCanvas.gameObject.SetActive(visible);
    }
    void LateUpdate()
    {
        Triangulate();
        //		hexMesh.Triangulate(cells);
        enabled = false;
    }

    public void Triangulate(HexCell[] cells)
    {
        terrain.Clear();
        //		hexMesh.Clear();
        //		vertices.Clear();
        //		colors.Clear();
        //		triangles.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        terrain.Apply();
        //		hexMesh.vertices = vertices.ToArray();
        //		hexMesh.colors = colors.ToArray();
        //		hexMesh.triangles = triangles.ToArray();
        //		hexMesh.RecalculateNormals();
        //		meshCollider.sharedMesh = hexMesh;
    }
}