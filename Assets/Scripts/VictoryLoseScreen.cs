using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Class that controls the Victory/Lose screen.
/// Fills it with content according to the match's result and provides method for a leaving button.
/// </summary>
public class VictoryLoseScreen : MonoBehaviour
{
    static VictoryLoseScreen instance;
    [SerializeField] GameObject clickBlocker;
    [SerializeField] GameObject victory;
    [SerializeField] GameObject Lose;
    [Space]
    [SerializeField] TMP_Text victoryText;
    [SerializeField] Image winnerIcon;

    private void Awake()
    {
        instance = this;
        victory.SetActive(false);
        Lose.SetActive(false);
        clickBlocker.SetActive(false);
    }

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;
    }

    public static void ShowVictory(List<PlayerValues> winners)
    {

        instance.victory.SetActive(true);
        instance.winnerIcon.sprite = winners[0].playerIcon;
        string winnerText = "Victory!";

        foreach (var player in winners)
        {
            winnerText += "\n" + player.Name;
        }

        instance.victoryText.text = winnerText;
        instance.clickBlocker.SetActive(true);
        instance.Lose.SetActive(false);
    }

    /// <summary>
    /// Attached to a button, this loads the main menu.
    /// </summary>
    public void LoadMenu()
    {
        Debug.Log("Loading Menu");
        SceneManager.LoadScene("MainMenu");
    }
}
