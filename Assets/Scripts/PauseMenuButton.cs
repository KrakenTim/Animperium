using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuButton : MonoBehaviour
{
    private void Awake()
    {
        PauseMenu.InPauseChanged += UpdateVisibility;
    }

    private void OnDestroy()
    {
        PauseMenu.InPauseChanged -= UpdateVisibility;
    }

    private void UpdateVisibility(bool inPause)
    {
        gameObject.SetActive(!inPause);
    }
}
