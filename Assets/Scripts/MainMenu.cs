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
        
        if (LoadingScreen.instance)
        LoadingScreen.instance.LoadScene("NormanMapGeneration");
        else
            SceneManager.LoadScene("NormanMapGeneration");
        Debug.Log("Game Start");
    }

    public void PlayOnline()
    {
        if (LoadingScreen.instance)
            LoadingScreen.instance.LoadScene("ServerClientChat");
        else
            SceneManager.LoadScene("ServerClientChat");
        Debug.Log("Start Online Play");
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