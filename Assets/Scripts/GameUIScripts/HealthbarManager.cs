using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarManager : MonoBehaviour
{
    private static HealthbarManager instance;

    [SerializeField] private HealthBar healthbarPrefab;

    [SerializeField] private Canvas healthBarCanvas;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    /// <summary>
    /// Creates a healthbar for given pawn and sets it up.
    /// </summary>
    public static HealthBar AddHealthbar(PlayerPawn pawn)
    {
        HealthBar newBar = Instantiate(instance.healthbarPrefab, instance.transform, false);

        newBar.Initialised(pawn, instance.healthBarCanvas);

        newBar.gameObject.name = pawn.name + " Healthbar";

        return newBar;
    }

    /// <summary>
    /// Removes healthbar of given pawn
    /// </summary>
    public static void RemoveHealthbar(PlayerPawn pawn)
    {
        if (pawn && pawn.healthBar && pawn.healthBar.gameObject)
        Destroy(pawn.healthBar.gameObject);
    }
}
