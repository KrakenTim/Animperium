using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexcellDecalManager : MonoBehaviour
{
    [System.Serializable]
    struct ActionTypeDecal
    {
        public ePlayeractionType actionType;
        public GameObject actionDecalPrefab;
    }

    [SerializeField] ActionTypeDecal[] decals;

    [SerializeField] Transform decalParent;

    Dictionary<ePlayeractionType, List<GameObject>> decalObjectPool = new Dictionary<ePlayeractionType, List<GameObject>>();

    List<GameObject> activeDecals = new List<GameObject>();

    [SerializeField] Vector3 decalOffset = new Vector3(0f, 0.1f, 0f);

    [SerializeField] float rotationOffset = 30f;

    bool allDeactivated;

    Vector3 usedDecalRotation = new Vector3(90f, 0f, 0f);

    private void OnEnable()
    {
        usedDecalRotation = new Vector3(90f, rotationOffset, 0f);

        GameInputManager.OnPawnSelected += UpdateDecals;
        InputMessageExecuter.ExecutedHexMessage += UpdateAfterInputMessageExecution;

        HexMapCamera.Instance.ChangedRotation += UpdateDecalRotation;
    }

    private void OnDisable()
    {
        GameInputManager.OnPawnSelected -= UpdateDecals;
        InputMessageExecuter.ExecutedHexMessage -= UpdateAfterInputMessageExecution;

        HexMapCamera.Instance.ChangedRotation -= UpdateDecalRotation;
    }

    /// <summary>
    /// Called after an InputMessage was executed, to update the decals.
    /// </summary>
    private void UpdateAfterInputMessageExecution(PlayerPawn actingPawn)
    {
        if (actingPawn == GameInputManager.SelectedPawn)
            UpdateDecals(actingPawn);
    }

    public void UpdateDecals(PlayerPawn selectedPawn)
    {
        if (!allDeactivated) DeactivateAllDecals();

        if (selectedPawn == null || selectedPawn.PlayerID != GameManager.LocalPlayerID) return;

        // TODO find more elegant solution
        HashSet<HexCell> neighbourCells = HexGridManager.Current.GetGrid(selectedPawn.HexCell).GetNeighbours(selectedPawn.HexCell, 1);

        bool hasRessourcesToBuild = selectedPawn.CanBuild
                                    && PlayerValueProvider.CanBuildAnything(selectedPawn.PlayerID, selectedPawn.PawnType);
        PlaceDecals(neighbourCells, hasRessourcesToBuild);
    }

    private void DeactivateAllDecals()
    {
        foreach (ePlayeractionType action in decalObjectPool.Keys)
        {
            foreach (GameObject decal in decalObjectPool[action])
            {
                decal.SetActive(false);

            }
        }

        activeDecals.Clear();
        allDeactivated = true;
    }

    private void PlaceDecals(HashSet<HexCell> neighbourCells, bool hasRessourcesToBuild)
    {
        Dictionary<ePlayeractionType, List<HexCell>> sortedCells = new Dictionary<ePlayeractionType, List<HexCell>>();

        foreach (HexCell cell in neighbourCells)
        {
            ePlayeractionType possibleAction = GameInputManager.PossibleAction(cell, hasRessourcesToBuild);

            if (possibleAction == ePlayeractionType.NONE) continue;

            if (!sortedCells.ContainsKey(possibleAction))
                sortedCells.Add(possibleAction, new List<HexCell>());

            sortedCells[possibleAction].Add(cell);
        }

        foreach (ePlayeractionType action in sortedCells.Keys)
        {
            PlaceDecals(action, sortedCells[action]);
        }
    }

    private void PlaceDecals(ePlayeractionType action, List<HexCell> cells)
    {
        if (!decalObjectPool.ContainsKey(action))
            decalObjectPool.Add(action, new List<GameObject>());

        int difference = cells.Count - decalObjectPool[action].Count;

        if (difference > 0)
        {
            GameObject decalPrefab = GetActionDecal(action);

            if (decalPrefab == null)
            {
                Debug.LogError($"No Prefab for {action} Found in {gameObject.name}\n", this);
                return;
            }

            for (int i = 0; i < difference; i++)
            {
                GameObject newDecal = Instantiate(decalPrefab, decalParent);

                decalObjectPool[action].Add(newDecal);
            }
        }

        for (int i = 0; i < cells.Count; i++)
        {
            decalObjectPool[action][i].transform.position = cells[i].WorldPosition + decalOffset;
            decalObjectPool[action][i].transform.localEulerAngles = usedDecalRotation;
            decalObjectPool[action][i].SetActive(true);

            activeDecals.Add(decalObjectPool[action][i]);
        }

        allDeactivated = false;
    }

    private GameObject GetActionDecal(ePlayeractionType searchedActionType)
    {
        foreach (ActionTypeDecal decalpair in decals)
        {
            if (decalpair.actionType == searchedActionType)
                return decalpair.actionDecalPrefab;
        }
        return null;
    }

    /// <summary>
    /// Updates the decals rotation according to camera.
    /// </summary>
    /// <param name="newCameraRotation">the camera's new Y rotation in degrees.</param>
    public void UpdateDecalRotation(float newCameraRotation)
    {
        newCameraRotation = Mathf.Round(newCameraRotation / 60f) * 60f;

        if (usedDecalRotation.y == newCameraRotation) return;
        usedDecalRotation.y = newCameraRotation + rotationOffset;

        foreach (var decal in activeDecals)
            decal.transform.localEulerAngles = usedDecalRotation;
    }
}