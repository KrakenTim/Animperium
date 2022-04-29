using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Pause menu providing the functionality of several buttons.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuUI;
    public static bool GameIsPaused = false;

   // public GameObject ui_canvas;
    GraphicRaycaster ui_raycaster;

    PointerEventData click_data;
    List<RaycastResult> click_results;

    public AudioSource ButtonSound;

    // Update is called once per frame
    void Update()
    {
       // ui_raycaster = ui_canvas.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();

        // New Input System
        Keyboard kb = InputSystem.GetDevice<Keyboard>();
        if (kb.escapeKey.wasPressedThisFrame)
        {
            {
                Resume();
            }
            
            {
                Pause();
            }
        }

        // Old Input System
      /*  if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        } */
    }

    void GetUiElementsClicked()
    {
        click_data.position = Mouse.current.position.ReadValue();
        click_results.Clear();

        ui_raycaster.Raycast(click_data, click_results);

        foreach(RaycastResult result in click_results)
        {
            GameObject ui_element = result.gameObject;
            Debug.Log(ui_element.name);
        }
    }
    
    public void Resume ()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        GameManager.ResignTroughQuitting();

        if (LoadingScreen.instance)
            LoadingScreen.instance.LoadScene("MainMenu");
        else
            SceneManager.LoadScene("MainMenu");

        Time.timeScale = 1f;
        GameIsPaused = false;

        Debug.Log("Loading Menu");
    }

    public void QuitGame()
    {
        GameManager.ResignTroughQuitting();

        Debug.Log("Game Close");
        Application.Quit();
    }

    public void playButtonSound()
    {
        ButtonSound.Play();
    }
}
