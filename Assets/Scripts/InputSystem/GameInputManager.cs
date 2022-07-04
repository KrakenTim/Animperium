using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the player's input.
/// </summary>
public class GameInputManager : MonoBehaviour
{
    static GameInputManager instance;

    public static event System.Action<PlayerPawn> OnPawnSelected;

    [SerializeField] PlayerPawn selectedPawn;
    public static PlayerPawn SelectedPawn => instance.selectedPawn;
    [SerializeField] HexCell selectedHexCell;
    public static HexCell SelectedHexCell => instance.selectedHexCell;

    [SerializeField] GameObject selectedPawnDecal;
    [SerializeField] float decalOffset = 0.1f;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
            SelectNextActivePawn();

        if (MouseOverHexCheck.IsOnHex)
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClickedOnHex(wasLeftClick: true);
            }
            else if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                ClickedOnHex(wasLeftClick: false);
            }
        }
    }

    private void ClickedOnHex(bool wasLeftClick)
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(inputRay, out RaycastHit hit)) return;

        selectedHexCell = HexGridManager.Current.GetHexCell(hit.point);

        if (IsPawnActionPossible(selectedHexCell))
        {
            if (wasLeftClick)
            {
                if (IsCollectPossible())
                {
                    InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Collect);
                    InputMessageExecuter.Send(message);
                    return;
                }

                if (IsSpawnPossible())
                {
                    InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Spawn);
                    InputMessageExecuter.Send(message);
                    return;
                }

                if (IsBuildingPossible())
                    InteractionMenuManager.OpenPawnCreationMenu(selectedHexCell);

                if (IsTunnelBuildingPossible())
                    InteractionMenuManager.OpenPawnCreationMenu(selectedHexCell);

            }
            else // rightClick
            {
                if (IsMovePossible())
                {
                    InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Move);
                    InputMessageExecuter.Send(message);
                    return;
                }
            }
        }
        else if (!wasLeftClick && IsDiggingPossible() && IsPawnActionPossible(SelectedHexCell, ignoreEnvironment: true))
        {
            InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Digging);
            InputMessageExecuter.Send(message);
            return;
        }
    }

    /// <summary>
    /// Checks if selected Pawn is Player Pawn, next to selected Cell and can act
    /// </summary>
    private bool IsPawnActionPossible(HexCell targetCell, int interactionRange = 1, bool ignoreEnvironment = false)
    {
        return GameManager.InputAllowed && selectedPawn != null && targetCell != null
            && selectedPawn.CanAct && selectedPawn.IsActivePlayerPawn
            && selectedPawn.HexCell.DistanceTo(targetCell) <= interactionRange

            // only check if character can step onto cell, if it's not a interaction that's possible at range.
            && (ignoreEnvironment || interactionRange > 1 || targetCell.CanMoveOnto(selectedPawn.HexCell));
    }

    private bool IsCollectPossible()
    {
        return selectedHexCell.Resource != null
               && selectedHexCell.Resource.CanCollect(selectedPawn) && selectedPawn.MP > 0;
    }

    private bool IsMovePossible()
    {
        return selectedPawn.IsUnit && selectedPawn.MP > 0
            && HexGridManager.IsWalkable(selectedHexCell);
    }

    private bool IsDiggingPossible()
    {
        return selectedPawn.CanDig && selectedPawn.MP > 0 && HexGridManager.IsUnderground(selectedHexCell) && selectedHexCell.IsDiggable;
    }

    private bool IsSpawnPossible()
    {
        // TODO(24.04.22) might check if needed resources are available and you have enough space in population to spawn.
        return !selectedHexCell.HasPawn && selectedPawn.Spawn != ePlayerPawnType.NONE
               && GameManager.PlayerPopulation(selectedPawn.PlayerID) < GameManager.PlayerPopulationMax(selectedPawn.PlayerID);
    }

    private bool IsBuildingPossible()
    {
        return selectedPawn.PawnType == ePlayerPawnType.Villager && !selectedHexCell.HasPawn && selectedHexCell.Resource == null
               && HexGridManager.IsSurface(selectedHexCell);
    }

    private bool IsTunnelBuildingPossible()
    {
        return selectedPawn.PawnType == ePlayerPawnType.Digger && !selectedHexCell.HasPawn && selectedHexCell.Resource == null;
    }

    private bool IsAttackPossible(PlayerPawn otherPawn)
    {
        return selectedPawn.AttackPower > 0 && otherPawn != null
           && selectedPawn.IsEnemyOf(otherPawn) && selectedPawn.InAttackRange(otherPawn);
    }

    private bool IsLearningPossible(PlayerPawn potentialSchool)
    {
        if (potentialSchool.PlayerID == selectedPawn.PlayerID && potentialSchool.PawnType.IsSchool()
            && selectedPawn.PawnData.IsLearnUpgradePossible)
            return true;

        return false;
    }

    private bool IsHealingPossible(PlayerPawn healTarget)
    {
        return selectedPawn.CanHeal && healTarget.IsWounded && healTarget.IsUnit && !selectedPawn.IsEnemyOf(healTarget);
    }

    private bool IsLayerSwitchPossible(PlayerPawn potentialTunnelEntry, out HexCell targetCell)
    {
        if (selectedPawn.MP == 0 || potentialTunnelEntry.PawnType != ePlayerPawnType.TunnelEntry
            || potentialTunnelEntry.IsEnemyOf(selectedPawn.PlayerID))
        {
            targetCell = null;
            return false;
        }

        targetCell = HexGridManager.Current.OtherLayerCell(selectedPawn.HexCell);
        return HexGridManager.IsWalkable(targetCell);
    }

    /// <summary>
    /// Called if a pawn was clicked.
    /// </summary>
    public static void ClickedOnPawn(PlayerPawn clickedPawn)
    {
        if (instance.selectedPawn && instance.IsPawnActionPossible(clickedPawn.HexCell, instance.selectedPawn.AttackRange))
        {

            // Check if enemy that might be attacked
            if (instance.IsAttackPossible(clickedPawn))
            {
                InputMessage message = InputMessageGenerator.CreateHexMessage(instance.selectedPawn, clickedPawn.HexCell,
                                                                              ePlayeractionType.Attack);
                InputMessageExecuter.Send(message);
            }
        }

        if (instance.IsPawnActionPossible(clickedPawn.HexCell))
        {
            if (instance.IsLayerSwitchPossible(clickedPawn, out HexCell targetCell))
            {
                InputMessage message = InputMessageGenerator.CreateHexMessage(instance.selectedPawn, clickedPawn.HexCell,
                                                                             ePlayeractionType.LayerSwitch);
                InputMessageExecuter.Send(message);
                return;
            }

            if (instance.IsHealingPossible(clickedPawn))
            {
                InputMessage message = InputMessageGenerator.CreateHexMessage(instance.selectedPawn, clickedPawn.HexCell,
                                                                              ePlayeractionType.Heal);
                InputMessageExecuter.Send(message);
                return;
            }

            // Check if school and upgrade possible
            if (instance.IsLearningPossible(clickedPawn))
            {
                InteractionMenuManager.OpenPawnCreationMenu(clickedPawn.HexCell, instance.selectedPawn.PawnData);
                return;
            }
        }

        instance.SelectPawn(clickedPawn);
    }

    private void SelectPawn(PlayerPawn newSelect)
    {
        if (newSelect.HP <= 0) return;

        // Select clicked Pawn
        selectedPawn = newSelect;

        if (selectedPawn != null)
        {
            selectedPawnDecal.SetActive(true);
            UpdateSelectedPawnDecal();
            selectedPawn.OnValueChange += UpdateSelectedPawnDecal;
        }
        else
            selectedPawnDecal.SetActive(false);

        OnPawnSelected?.Invoke(newSelect);
    }

    /// <summary>
    /// Deselects currently selected pawn.
    /// If a specific pawn is given it only deselects the currently selected pawn if it's equal to the given pawn.
    /// </summary>
    public static void DeselectPawn(PlayerPawn pawnToDeselect = null)
    {
        if (pawnToDeselect != null && pawnToDeselect != SelectedPawn) return;

        if (instance.selectedPawn != null)
            instance.selectedPawn.OnValueChange -= instance.UpdateSelectedPawnDecal;

        instance.selectedPawn = null;
        instance.selectedPawnDecal.SetActive(false);

        OnPawnSelected?.Invoke(null);
    }

    private void UpdateSelectedPawnDecal()
    {
        if (!selectedPawn) return;

        Vector3 position = selectedPawn.HexCell.transform.position;
        position.y += decalOffset;
        selectedPawnDecal.transform.position = position;
    }

    private void SelectNextActivePawn()
    {
        HexGridLayer cameraLayer = HexMapCamera.GridLayer;

        if (selectedPawn == null || selectedPawn.PlayerID != GameManager.LocalPlayerID
            || selectedPawn.HexCell.gridLayer != cameraLayer)
        {
            if (SelectNearest(HexMapCamera.LocalPosition, cameraLayer))
            {
                HexMapCamera.SetPosition(selectedPawn.WorldPosition);
                return;
            }
        }

        if (!GameManager.PlayerValueProvider.TryGetPlayerValues(GameManager.LocalPlayerID, out PlayerValues player))
            return;

        int pos = player.ownedPawns.IndexOf(selectedPawn);
        int count = player.ownedPawns.Count;

        for (int i = 1; i < count; i++)
        {
            PlayerPawn nextPawn = player.ownedPawns[(pos + i) % count];
            if (IsUnitCanActAndIsOnLayer(nextPawn, cameraLayer))
            {
                SelectPawn(nextPawn);
                HexMapCamera.SetPosition(selectedPawn.WorldPosition);
                return;
            }
        }

        HexMapCamera.SwapUsedGrid();
        if (SelectNearest(HexMapCamera.LocalPosition, HexMapCamera.GridLayer))
            HexMapCamera.SetPosition(selectedPawn.WorldPosition);
        else
            HexMapCamera.SwapUsedGrid();
    }

    private bool SelectNearest(Vector3 position, HexGridLayer layer)
    {
        if (GameManager.PlayerValueProvider.TryGetPlayerValues(GameManager.LocalPlayerID, out PlayerValues player))
        {
            PlayerPawn nearestPawn = null;
            float nextDistance;
            float shortestDistance = float.MaxValue;

            foreach (PlayerPawn pawn in player.ownedPawns)
            {
                if (!IsUnitCanActAndIsOnLayer(pawn, layer)) continue;

                nextDistance = Vector3.Distance(pawn.WorldPosition, position);

                if (nextDistance < shortestDistance)
                {
                    shortestDistance = nextDistance;
                    nearestPawn = pawn;
                }
            }

            if (nearestPawn != null)
            {
                SelectPawn(nearestPawn);
                return true;
            }
        }
        return false;
    }

    private static bool IsUnitCanActAndIsOnLayer(PlayerPawn pawn, HexGridLayer layer)
    {
        return pawn.CanAct && pawn.HexCell.gridLayer == layer && !pawn.PawnType.IsBuilding();
    }
}
