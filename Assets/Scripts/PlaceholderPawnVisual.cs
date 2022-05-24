using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// fills the Texts below itself matching to the type of the Pawn it belongs to.
/// </summary>
public class PlaceholderPawnVisual : MonoBehaviour
{
    [SerializeField] float unitSize = 7f;

    void Start()
    {
        // Update Name visual

        PlayerPawn myPawn = GetComponentInParent<PlayerPawn>();
        if (myPawn == null) return;

        // Update Text
        TMPro.TMP_Text[] myTexts = GetComponentsInChildren<TMPro.TMP_Text>();
        foreach (var textfield in myTexts)
            textfield.text = myPawn.FriendlyName;

        // Update Size
        if (myPawn.IsUnit)
        {
            float scale = unitSize / transform.GetChild(0).localScale.x;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform next = transform.GetChild(i);
                next.localScale *= scale;
                next.localPosition *= scale;
            }
        }
    }
}