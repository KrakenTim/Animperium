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
    const bool MESSE_VERSION = true;

    public AudioSource ButtonSound;

    [SerializeField] PersistingMatchData currentMatchData;

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
    }

    public void StartGame()
    {
        if (MESSE_VERSION)
        {
            currentMatchData.MapPath = Path.Combine(AI_File.PathTempMaps, AI_File.NameMesseMap);

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