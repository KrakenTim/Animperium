using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnTimer : MonoBehaviour
{
    [SerializeField] TMP_Text turnTimer;
    public int maxSecondsPerTurn = 90;
    public int remainingSeconds;
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

            turnTimer.text = "Turn Time: " + remainingSeconds.ToString();
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
        turnTimer.text = "Turn Time: " + remainingSeconds.ToString();
    }
}
