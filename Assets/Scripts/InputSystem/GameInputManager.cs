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
    [SerializeField] HexcellDecalManager hexcellDecalManager;
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
        {
            if (Keyboard.current.shiftKey.isPressed)
                SelectNextActivePawn(previous: true);
            else
                SelectNextActivePawn(previous: false);
        }

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

        bool isMenuOpen = false;
        if (IsPawnActionPossible(selectedHexCell))
        {
            if (wasLeftClick)
            {
                if (IsCollectPossible(selectedHexCell))
                {
                    InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Collect);
                    InputMessageExecuter.Send(message);
                    return;
                }

                if (IsSpawnPossible(selectedHexCell))
                {
                    InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Spawn);
                    InputMessageExecuter.Send(message);
                    return;
                }

                if (IsBuildingPossible(selectedHexCell))
                {
                    InteractionMenuManager.OpenPawnCreationMenu(selectedHexCell);
                    isMenuOpen = true;
                }

                if (IsTunnelBuildingPossible(selectedHexCell))
                {
                    InteractionMenuManager.OpenPawnCreationMenu(selectedHexCell);
                    isMenuOpen = true;
                }
            }
            else // rightClick
            {
                if (IsCollectPossible(selectedHexCell))
                {
                    InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Collect);
                    InputMessageExecuter.Send(message);
                    return;
                }

                if (IsMovePossible(selectedHexCell))
                {
                    InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Move);
                    InputMessageExecuter.Send(message);
                    return;
                }
            }
        }
        else if (!wasLeftClick && IsDiggingPossible(selectedHexCell) && IsPawnActionPossible(SelectedHexCell, ignoreEnvironment: true))
        {
            InputMessage message = InputMessageGenerator.CreateHexMessage(selectedPawn, selectedHexCell, ePlayeractionType.Digging);
            InputMessageExecuter.Send(message);
            return;
        }

        if (!isMenuOpen)
            DeselectPawn();
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

    private bool IsCollectPossible(HexCell cell)
    {
        return cell.Resource != null
               && cell.Resource.CanCollect(selectedPawn) && selectedPawn.MP > 0;
    }

    private bool IsMovePossible(HexCell cell)
    {
        return SelectedPawn.IsUnit && selectedPawn.MP > 0
            && HexGridManager.IsWalkable(cell);
    }

    private bool IsDiggingPossible(HexCell cell)
    {
        return selectedPawn.CanDig && selectedPawn.MP > 0 && HexGridManager.IsUnderground(cell) && cell.IsDiggable;
    }

    private bool IsSpawnPossible(HexCell cell)
    {
        // TODO(24.04.22) might check if needed resources are available and you have enough space in population to spawn.
        return !cell.HasPawn && !cell.HasResource && selectedPawn.Spawn != ePlayerPawnType.NONE
               && GameManager.PlayerPopulation(selectedPawn.PlayerID) < GameManager.PlayerPopulationMax(selectedPawn.PlayerID);
    }

    private bool IsBuildingPossible(HexCell cell)
    {
        return selectedPawn.PawnType == ePlayerPawnType.Villager && !cell.HasPawn && !cell.HasResource
               && HexGridManager.IsSurface(cell);
    }

    private bool IsTunnelBuildingPossible(HexCell cell)
    {
        return selectedPawn.PawnType == ePlayerPawnType.Digger && !cell.HasPawn && !cell.HasResource;
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

        Vector3 position = selectedPawn.HexCell.ObjectPosition;
        position.y += decalOffset;
        selectedPawnDecal.transform.position = position;

        hexcellDecalManager.UpdateDecals(selectedPawn);
    }

    private void SelectNextActivePawn(bool previous = false)
    {
        HexGridLayer cameraLayer = HexMapCamera.GridLayer;

        if (selectedPawn == null || selectedPawn.PlayerID != GameManager.LocalPlayerID
            || selectedPawn.HexCell.gridLayer != cameraLayer)
        {
            if (SelectNearest(HexMapCamera.Position, cameraLayer))
            {
                HexMapCamera.SetPosition(selectedPawn.WorldPosition);
                return;
            }
        }

        if (!GameManager.PlayerValueProvider.TryGetPlayerValues(GameManager.LocalPlayerID, out PlayerValues player))
            return;

        int pos = player.ownedPawns.IndexOf(selectedPawn);
        PlayerPawn newSelectedPawn;

        if (previous)
            newSelectedPawn = SelectPreviousInPawnList(pos, player.ownedPawns, cameraLayer);
        else
            newSelectedPawn = SelectNextInPawnList(pos, player.ownedPawns, cameraLayer);

        if (newSelectedPawn != null)
        {
            SelectPawn(newSelectedPawn);
            HexMapCamera.SetPosition(selectedPawn.WorldPosition);
            return;
        }

        HexMapCamera.SwapUsedGrid();
        if (SelectNearest(HexMapCamera.Position, HexMapCamera.GridLayer))
            HexMapCamera.SetPosition(selectedPawn.WorldPosition);
        else
            HexMapCamera.SwapUsedGrid();
    }

    /// <summary>
    /// Helper function, return the next unit on the layer that might still act
    /// </summary>
    private PlayerPawn SelectNextInPawnList(int pos, List<PlayerPawn> pawns, HexGridLayer layer)
    {
        int count = pawns.Count;
        for (int i = 1; i < count; i++)
        {
            PlayerPawn nextPawn = pawns[(pos + i) % count];

            if (IsUnitCanActAndIsOnLayer(nextPawn, layer))
                return nextPawn;
        }
        return null;
    }

    /// <summary>
    /// Helper function, return the previous unit on the layer that might still act
    /// </summary>
    private PlayerPawn SelectPreviousInPawnList(int pos, List<PlayerPawn> pawns, HexGridLayer layer)
    {
        int count = pawns.Count;
        for (int i = count - 1; i > 0; i--)
        {
            PlayerPawn nextPawn = pawns[(pos + i) % count];

            if (IsUnitCanActAndIsOnLayer(nextPawn, layer))
                return nextPawn;
        }
        return null;
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

    public static ePlayeractionType PossibleAction(HexCell cell)
    {
        if (SelectedPawn == null || cell == null) return ePlayeractionType.NONE;

        if (cell.HasPawn)
        {
            if (instance.IsAttackPossible(cell.Pawn))
                return ePlayeractionType.Attack;

            if (instance.IsLayerSwitchPossible(cell.Pawn, out HexCell unused))
                return ePlayeractionType.LayerSwitch;

            if (instance.IsHealingPossible(cell.Pawn))
                return ePlayeractionType.Heal;

            if (instance.IsLearningPossible(cell.Pawn))
                return cell.Pawn.IsUnit ? ePlayeractionType.UnitUpgrade : ePlayeractionType.BuildingUpgrade;
        }

        if (instance.IsCollectPossible(cell))
            return ePlayeractionType.Collect;

        if (instance.IsSpawnPossible(cell))
            return ePlayeractionType.Spawn;

        if (instance.IsBuildingPossible(cell))
            return ePlayeractionType.Build;

        if (instance.IsTunnelBuildingPossible(cell))
            return ePlayeractionType.Build;

        if (instance.IsDiggingPossible(cell))
            return ePlayeractionType.Digging;

        if (instance.IsMovePossible(cell))
            return ePlayeractionType.Move;

        return ePlayeractionType.NONE;
    }
}