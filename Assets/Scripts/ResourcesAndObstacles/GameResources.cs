using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Struct used to represent a player's collected resurces, the price of units etc.
/// </summary>
[System.Serializable]
public struct GameResources
{
    public int food;
    public int wood;
    public int ore;

    public int this[eResourceType resource]
    {
        get
        {
            switch (resource)
            {
                case eResourceType.Food:
                    return food;
                case eResourceType.Wood:
                    return wood;
                case eResourceType.Ore:
                    return ore;

                default:
                    Debug.LogError($"[GameResources] UNDEFINED for {resource}");
                    return 0;
            }
        }
        set
        {
            switch (resource)
            {
                case eResourceType.Food:
                    food = value;
                    break;
                case eResourceType.Wood:
                    wood = value;
                    break;
                case eResourceType.Ore:
                    ore = value;
                    break;

                default:
                    Debug.LogError($"[GameResources] UNDEFINED for {resource}");
                    break;
            }
        }
    }
}
