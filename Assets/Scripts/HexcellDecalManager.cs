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

    [SerializeField] Vector3 decalOffset = new Vector3(0f, 0.1f, 0f);

    [SerializeField] float rotationOffset = 30f;

    bool allDeactivated;

    private void OnEnable()
    {
        GameInputManager.OnPawnSelected += UpdateDecals;
        InputMessageExecuter.ExecutedHexMessage += UpdateAfterInputMessageExecution;
    }

    private void OnDisable()
    {
        GameInputManager.OnPawnSelected -= UpdateDecals;
        InputMessageExecuter.ExecutedHexMessage -= UpdateAfterInputMessageExecution;
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

                Vector3 rotation = newDecal.transform.localEulerAngles;
                rotation.y += rotationOffset;
                newDecal.transform.localEulerAngles = rotation;
            }
        }

        for (int i = 0; i < cells.Count; i++)
        {
            decalObjectPool[action][i].transform.position = cells[i].WorldPosition + decalOffset;
            decalObjectPool[action][i].SetActive(true);
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
}