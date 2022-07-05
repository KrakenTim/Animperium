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
    /// <summary>
    /// Seeks instance in Scene if it was set yet
    /// </summary>
    static VictoryLoseScreen Instance
    {
        get
        { 
            if (instance == null)
                instance = FindObjectOfType<VictoryLoseScreen>(true);
            return instance;
        }
    }

    [SerializeField] GameObject clickBlocker;
    [SerializeField] GameObject victory;
    [SerializeField] GameObject Lose;
    [Space]
    [SerializeField] TMP_Text victoryText;
    [SerializeField] ColorableImage winnerIcon;

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
        Instance.victory.SetActive(true);

        if (!instance.victory.activeInHierarchy)
            Debug.LogError("Victory Screen not visible is Canvas active?\n");

        instance.winnerIcon.SetPlayer(winners[0]);
        string winnerText = "Victory!";

        foreach (var player in winners)
        {
            winnerText += "\n" + player.name;
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
        SceneManager.LoadScene(AI_Scene.SCENENAME_MainMenu);
    }
}
