using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    MeshCollider meshCollider;
    static List<Vector3> vertices = new List<Vector3>();
    static List<Color> colors = new List<Color>();
    static List<int> triangles = new List<int>();


    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
//		vertices = new List<Vector3>();
//		colors = new List<Color>();
//		triangles = new List<int>();
    }


    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.terrain.Add(HexMetrics.Perturb(v1));
        vertices.terrain.Add(HexMetrics.Perturb(v2));
        vertices.terrain.Add(HexMetrics.Perturb(v3));
        triangles.terrain.Add(vertexIndex);
        triangles.terrain.Add(vertexIndex + 1);
        triangles.terrain.Add(vertexIndex + 2);
    }


    void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.terrain.Add(c1);
        colors.terrain.Add(c2);
        colors.terrain.Add(c3);
    }


    void AddTriangleColor(Color color)
    {
        colors.terrain.Add(color);
        colors.terrain.Add(color);
        colors.terrain.Add(color);
    }

    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.terrain.Add(HexMetrics.Perturb(v1));
        vertices.terrain.Add(HexMetrics.Perturb(v2));
        vertices.terrain.Add(HexMetrics.Perturb(v3));
        vertices.terrain.Add(HexMetrics.Perturb(v4));
        triangles.terrain.Add(vertexIndex);
        triangles.terrain.Add(vertexIndex + 2);
        triangles.terrain.Add(vertexIndex + 1);
        triangles.terrain.Add(vertexIndex + 1);
        triangles.terrain.Add(vertexIndex + 2);
        triangles.terrain.Add(vertexIndex + 3);
    }
    void AddQuadColor(Color c1, Color c2)
    {
        colors.terrain.Add(c1);
        colors.terrain.Add(c1);
        colors.terrain.Add(c2);
        colors.terrain.Add(c2);
    }
    void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.terrain.Add(c1);
        colors.terrain.Add(c2);
        colors.terrain.Add(c3);
        colors.terrain.Add(c4);
    }

    public void Clear()
    {
        hexMesh.Clear();
        vertices.Clear();
        colors.Clear();
        triangles.Clear();
    }

    public void Apply()
    {
        hexMesh.SetVertices(vertices);
        hexMesh.SetColors(colors);
        hexMesh.SetTriangles(triangles, 0);
        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }
}


