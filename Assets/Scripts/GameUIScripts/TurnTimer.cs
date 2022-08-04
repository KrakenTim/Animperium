using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TurnTimer : MonoBehaviour
{
    [SerializeField] TMP_Text inGameTurn;
    [Space]
    [SerializeField] TMP_Text turnTimeMinute;
    [SerializeField] TMP_Text turnTimeSecond;
    [Space]
    [SerializeField] ColorableImage activePlayerIcon;

    public int maxSecondsPerTurn = 90;
    public int remainingSeconds = 90;
    float secondTick;

    private void Awake()
    {
        GameManager.TurnStarted += ResetTimer;
    }

    private void OnDestroy()
    {
        GameManager.TurnStarted -= ResetTimer;
    }

    // Update is called once per frame
    void Update()
    {
        secondTick += Time.deltaTime;
        if (secondTick >= 1f)
        {
            secondTick -= 1;

            DeductSecond();
        }
    }

    void DeductSecond()
    {
        remainingSeconds -= 1;
        UpdateTimerVisual();

        if (remainingSeconds == 0)
        {
            enabled = false; // Update is only called, if component is enabled.

            if (!OnlineGameManager.IsOnlineGame || GameManager.LocalPlayerID == GameManager.ActivePlayerID)
            {
                var message = InputMessageGenerator.CreateBasicMessage(ePlayeractionType.EndTurn);
                InputMessageExecuter.Send(message);
            }
        }
    }

    private void ResetTimer(int unusedPlayerID)
    {
        remainingSeconds = maxSecondsPerTurn;

        if (inGameTurn)
            inGameTurn.text = GameManager.Turn.ToString();

        if (activePlayerIcon)
            activePlayerIcon.SetPlayer(GameManager.ActivePlayerID);

        enabled = true;
        secondTick = 0;

        UpdateTimerVisual();
    }

    /// <summary>
    /// Updates the text field in the UI for the turn timer.
    /// </summary>
    private void UpdateTimerVisual()
    {
        //turnTimeCounter.text = remainingSeconds.ToString();
        turnTimeMinute.text = (remainingSeconds / 60).ToString();
        int seconds = remainingSeconds % 60;
        turnTimeSecond.text = (seconds < 10 ? "0" : "") + seconds.ToString();
    }
}
