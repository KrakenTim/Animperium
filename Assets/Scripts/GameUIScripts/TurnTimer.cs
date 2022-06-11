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
    public bool deductingTime;

    private void Awake()
    {
        GameManager.TurnStarted += ResetTimer;
    }

    private void OnDiestroy()
    {
        GameManager.TurnStarted -= ResetTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (deductingTime == false)
        {
            deductingTime = true;
            StartCoroutine(DeductSeconds());

            UpdateTimerVisual();
        }
    }

    IEnumerator DeductSeconds()
    {
        yield return new WaitForSeconds(1);

        if (remainingSeconds <= 0)
        {

            if (!OnlineGameManager.IsOnlineGame || GameManager.LocalPlayerID == GameManager.ActivePlayerID)
            {
                var message = InputMessageGenerator.CreateBasicMessage(ePlayeractionType.EndTurn);
                InputMessageExecuter.Send(message);
            }
            yield break;
        }

        remainingSeconds -= 1;

        deductingTime = false;
    }

    private void ResetTimer(int unusedPlayerID)
    {
        remainingSeconds = maxSecondsPerTurn;

        inGameTurn.text = GameManager.Turn.ToString();

        activePlayerIcon.SetPlayer(GameManager.ActivePlayerID);

        deductingTime = false;

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
