using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// fills the Texts below itself matching to the type of the Pawn it belongs to.
/// </summary>
public class PlaceholderPawnVisual : MonoBehaviour
{
    void Start()
    {
        // Update Name visual

        PlayerPawn myPawn = GetComponentInParent<PlayerPawn>();
        if (myPawn == null) return;

        // Update Text
        TMPro.TMP_Text[] myTexts = GetComponentsInChildren<TMPro.TMP_Text>();
        foreach (var textfield in myTexts)
            textfield.text = myPawn.FriendlyName;
    }
}