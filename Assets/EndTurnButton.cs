using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    private Button endTurnButton;

    private void Awake()
    {
        endTurnButton = GetComponent<Button>();
        GameManager.TurnStarted += UpdateVisibility;
    }

    private void OnDestroy()
    {
        GameManager.TurnStarted -= UpdateVisibility;
    }

    public void Button_EndTurn()
    {
        if (!GameManager.InputAllowed) return;

        endTurnButton.interactable = false;

        var message = InputMessageGenerator.CreateBasicMessage(ePlayeractionType.EndTurn);
        InputMessageExecuter.Send(message);
    }

    private void UpdateVisibility(int activePlayerID)
    {
        bool shouldBeActive = activePlayerID == GameManager.LocalPlayerID;

        endTurnButton.interactable = shouldBeActive;
        gameObject.SetActive(shouldBeActive);
    }
}
