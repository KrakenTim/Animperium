using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioSource ButtonSound;

    public void StartGame()
    {
        SceneManager.LoadScene("NormansTestScene");
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