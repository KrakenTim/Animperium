using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides methods for the buttons in the main menu.
/// </summary>
public class MainMenu : MonoBehaviour
{
    // Set MESSE_VERSION of true when we Presentate our Game by Events like Gamescom, Gamesweek etc.
    public const bool MESSE_VERSION = false;

    public AudioSource ButtonSound;

    [SerializeField] PersistingMatchData currentMatchData;

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
    }

    public void StartGame()
    {
        // For Events like Gamescom, Gamesweek etc.
        if (MESSE_VERSION)
        {
#pragma warning disable CS0162 // Unerreichbarer Code wurde entdeckt.
            currentMatchData.MapPath = Path.Combine(AI_File.PathTempMaps, AI_File.NameMesseMap);
#pragma warning restore CS0162 // Unerreichbarer Code wurde entdeckt.

            AI_Scene.LoadSceneWithLoadingScreen(AI_Scene.SCENENAME_Game);
            return;
        }


        SceneManager.LoadScene(AI_Scene.SCENENAME_GamePreparation);
        Debug.Log("Game Start");
    }

    public void PlayOnline()
    {
        //if (LoadingScreen.instance)
        //    LoadingScreen.instance.LoadScene("ServerClientChat");
        //else
        SceneManager.LoadScene(AI_Scene.SCENENAME_OnlineRoom);

        Debug.Log("Start Online Play");
    }

    public void OpenMapEditor()
    {
        currentMatchData.MapPath = Path.Combine(AI_File.PathTempMaps, AI_File.NameEditorMap);

        if (!currentMatchData.IsMapPathValid)
            currentMatchData.MapPath = null;

        if (LoadingScreen.instance)
            LoadingScreen.instance.LoadScene(AI_Scene.SCENENAME_MapEditor);
        else
            SceneManager.LoadScene(AI_Scene.SCENENAME_MapEditor);

        Debug.Log("Open Map Editor");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stops Playmode
#endif

        Application.Quit();
        Debug.Log("Game Close");
    }

    public void playButtonSound()
    {
        ButtonSound.Play();
    }
}