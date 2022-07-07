using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorMouseArea : MonoBehaviour
{
    [SerializeField, Min(1f)] float ticksPerSecond = 20;
    [SerializeField] GameObject decalPrefab;

    int usedBrushSize = 1;
    int neededDecals;

    bool decalsActive = true;

    GameObject[] decals = new GameObject[0];

    HexCell lastCell;
    [Space]
    [SerializeField] HexGrid hexGrid;
    [SerializeField] HexMapEditor hexMapEditor;
    [SerializeField] float decalOffset = 0.1f;

    private void OnEnable()
    {
        HexMapEditor.BrushSizeChanged += UpdateSize;

        StartCoroutine(CheckMousePosition());
    }

    private void Start()
    {
        UpdateSize(hexMapEditor.brushSize);
    }

    private void OnDisable()
    {
        HexMapEditor.BrushSizeChanged -= UpdateSize;
    }

    private IEnumerator CheckMousePosition()
    {
        yield return new WaitForSeconds(1f / ticksPerSecond);

        UpdatePosition();

        StartCoroutine(CheckMousePosition());
    }

    private void UpdatePosition(bool enforce = false)
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (HexMapCamera.Locked || !Physics.Raycast(inputRay, out hit))
        {
            DeactivateAll();
            return;
        }

        HexCell newCenter = hexGrid.GetCell(hit.point);

        if (!enforce && newCenter && newCenter == lastCell) return;

        HashSet<HexCell> coveredCells = hexGrid.GetNeighbours(newCenter, usedBrushSize, withCenter: true);
        
        int counter = 0;
        foreach (var cell in coveredCells)
        {
            Vector3 pos = cell.transform.position;
            pos.y += decalOffset;
            decals[counter].transform.position = pos;

            decals[counter].SetActive(true);
            counter += 1;
        }

        for (int i = counter + 1; i < neededDecals; i++)
        {
            decals[i].SetActive(false);
        }

        decalsActive = true;
    }

    private void UpdateSize(int newSize)
    {
        usedBrushSize = newSize;

        neededDecals = 1 + 6 * (newSize * (newSize + 1)) / 2;

        if (decals.Length < neededDecals)
        {
            GameObject[] newDecals = new GameObject[neededDecals];

            for (int i = 0; i < decals.Length; i++)
                newDecals[i] = decals[i];

            for (int i = decals.Length; i < neededDecals; i++)
            {
                GameObject newDecal = Instantiate(decalPrefab, this.transform);
                newDecals[i] = newDecal;
            }

            decals = newDecals;
        }
        else if (decals.Length > neededDecals)
        {
            for (int i = neededDecals; i < decals.Length; i++)
                decals[i].SetActive(false);
        }

        UpdatePosition(enforce: true);
    }

    private void DeactivateAll()
    {
        if (!decalsActive) return;

        for (int i = 0; i < neededDecals; i++)
            decals[i].SetActive(false);

        decalsActive = false;
    }
}
