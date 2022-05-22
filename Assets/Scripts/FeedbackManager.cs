using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Vfx and Sound Feedback for player actions
/// </summary>
public class FeedbackManager : MonoBehaviour
{
    private static FeedbackManager instance;

    [System.Serializable]
    public class PlayeractionFeedback
    {
        public GameObject vfxStarter;
        public AudioClip sfxTarget;
        public GameObject vfxTarget;

        public void FillEmpty(PlayeractionFeedback filler)
        {
            if (vfxStarter == null)
                vfxStarter = filler.vfxStarter;
            if (sfxTarget == null)
                sfxTarget = filler.sfxTarget;
            if (vfxTarget == null)
                vfxTarget = filler.vfxTarget;
        }
    }

    [SerializeField] AudioSource simpleAudioSource;

    [Header("Player Action Feedback")]
    [SerializeField] PlayeractionFeedback attackPhysical;
    [SerializeField] PlayeractionFeedback attackMagical;
    [SerializeField] PlayeractionFeedback build;
    [SerializeField] PlayeractionFeedback buildingUpgrade;
    [SerializeField] PlayeractionFeedback collect;
    [SerializeField] PlayeractionFeedback heal;
    [SerializeField] PlayeractionFeedback move;
    [SerializeField] PlayeractionFeedback spawn;
    [SerializeField] PlayeractionFeedback unitUpgrade;

    [Header("World Action Feedback")]
    [SerializeField] PlayeractionFeedback unitKilled;
    [SerializeField] PlayeractionFeedback buildingDestroyed;

    [Space]
    [SerializeField] PlayeractionFeedback fallbackPlaceholder;


    private void Awake()
    {
        instance = this;

        FillEmptyPlayerFeedback();
    }

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;
    }

    /// <summary>
    /// Plays the feedback for the given action by the player on the Hex Grid.
    /// </summary>
    public static void PlayHexActionFeedback(PlayerPawn actingPawn, HexCell target, ePlayeractionType action)
    {
        if (!action.IsOnHexGrid()) return;

        PlayeractionFeedback usedFeedBack = instance.GetPlayeractionFeedback(action, actingPawn);

        instance.PlayFeedback(usedFeedBack);
    }

    public static void PlayPawnDestroyed(PlayerPawn destroyedPawn)
    {
        if (destroyedPawn.IsBuilding)
            instance.PlayFeedback(instance.buildingDestroyed);
        else
            instance.PlayFeedback(instance.unitKilled);
    }

    private void PlayFeedback(PlayeractionFeedback playedFeedback)
    {
        simpleAudioSource.PlayOneShot(playedFeedback.sfxTarget);
    }

    /// <summary>
    /// Returns the player feedback for the given player action.
    /// </summary>
    private PlayeractionFeedback GetPlayeractionFeedback(ePlayeractionType action, PlayerPawn actingPawn)
    {
        switch (action)
        {
            case ePlayeractionType.Move:
                return move;
            case ePlayeractionType.Attack:
                if (actingPawn.IsMagicUser)
                    return attackMagical;
                else
                    return attackPhysical;

            case ePlayeractionType.Collect:
                return collect;
            case ePlayeractionType.Spawn:
                return spawn;
            case ePlayeractionType.UnitUpgrade:
                return unitUpgrade;
            case ePlayeractionType.Build:
                return build;
            case ePlayeractionType.BuildingUpgrade:
                return buildingUpgrade;
            case ePlayeractionType.Heal:
                return heal;

            default:
                Debug.LogError($"Feedback UNDEFINED for {action}\n", this);
                return fallbackPlaceholder;
        }

    }

    /// <summary>
    /// Fills feedback for player actions width fallbackplaceholder values if field is empty.
    /// </summary>
    private void FillEmptyPlayerFeedback()
    {
        foreach (ePlayeractionType action in System.Enum.GetValues(typeof(ePlayeractionType)))
        {
            if (action.IsOnHexGrid())
            {
                if (action == ePlayeractionType.Attack)
                
                {
                    attackPhysical.FillEmpty(fallbackPlaceholder);
                    attackMagical.FillEmpty(fallbackPlaceholder);
                    continue;
                }

                PlayeractionFeedback nextFeedback = GetPlayeractionFeedback(action, null);

                nextFeedback.FillEmpty(fallbackPlaceholder);
            }
        }

        buildingDestroyed.FillEmpty(fallbackPlaceholder);
        unitKilled.FillEmpty(fallbackPlaceholder);
    }
}
