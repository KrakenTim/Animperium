using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Stores the map after it was changed, allows to load previous state
/// </summary>
public class UndoSystem : MonoBehaviour
{
    [SerializeField] KeyCode alternativeGoBackKey = KeyCode.U;

    [SerializeField] int amount = 10;

    [SerializeField] HexGrid grid;

    List<int> backups = new List<int>();

    private string MapName(int index) => $"Auto{index}.map";

    private Coroutine coroutine;

    private bool ignoreNext;

    private void OnEnable()
    {
        HexGridChunk.OnTriangulate += HexGridChangeRunning;
    }

    private void OnDisable()
    {
        HexGridChunk.OnTriangulate -= HexGridChangeRunning;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(alternativeGoBackKey) && !SaveLoadMenu.menuOpen) || Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
            Undo();
    }

    private void HexGridChangeRunning()
    {
        if (coroutine == null)
            coroutine = StartCoroutine(ChangeHappended());
    }

    private IEnumerator ChangeHappended()
    {
        yield return new WaitForEndOfFrame();

        if (ignoreNext)
        {
            ignoreNext = false;
            coroutine = null;
            yield break;
        }

        int usedID = 0;
        while (usedID < amount && backups.Contains(usedID))
            usedID++;

        if (usedID == amount)
        {
            usedID = backups.Last();
            backups.Remove(usedID);
        }

        SaveLoadMenu.Save(Path.Combine(AI_File.PathTempMaps, MapName(usedID)), grid);

        backups.Insert(0, usedID);

        coroutine = null;

        //Debug.Log($"SAVE {MapName(usedID)}, others: " + string.Join(",", backups.ToArray()));
    }

    public void Undo()
    {
        if (backups.Count <= 2) return;

        ignoreNext = true;

        backups.Remove(backups[0]); // remove the current from the first position

        Debug.Log($"[{GetType().Name}] Load {MapName(backups[0])}, others: " + string.Join(",", backups.ToArray()));
        SaveLoadMenu.Load(Path.Combine(AI_File.PathTempMaps, MapName(backups[0])), grid);
    }
}