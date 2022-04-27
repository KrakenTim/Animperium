using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnTimer : MonoBehaviour
{
    [SerializeField] TMP_Text turnTimer;
    public int seconds = 90;
    public bool deductingTime;

    // Update is called once per frame
    void Update()
    {
       if (deductingTime == false)
        {
            deductingTime = true;
            StartCoroutine(DeductSeconds());

            turnTimer.text = "Turn Time: " + seconds.ToString();
        }
    }

    IEnumerator DeductSeconds()
    {
        yield return new WaitForSeconds(1);
        seconds -= 1;

        if (seconds <= 0)
        GameManager.EndTurn();

        if (seconds <= 0)
            seconds = 90;

        deductingTime = false;
    }
}
