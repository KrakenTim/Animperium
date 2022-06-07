using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnTimer : MonoBehaviour
{
    [SerializeField] TMP_Text inGameTurn;
    [Space]
    [SerializeField] TMP_Text turnTimeMinute;
    [SerializeField] TMP_Text turnTimeSecond;

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
        remainingSeconds -= 1;

        if (remainingSeconds <= 0)
            GameManager.EndTurn();

        deductingTime = false;
    }

    private void ResetTimer(int unusedPlayerID)
    {
        remainingSeconds = maxSecondsPerTurn;

        inGameTurn.text = GameManager.Turn.ToString();

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
