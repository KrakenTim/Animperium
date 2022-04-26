using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides methods for the buttons in the main menu.
/// </summary>
public class MainMenu : MonoBehaviour
{
    public AudioSource ButtonSound;

    public void StartGame()
    {
        SceneManager.LoadScene("NormanMapGeneration");
        Debug.Log("Game Start");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Close");
    }

    public void playButtonSound()
    {
        ButtonSound.Play();
    }
}