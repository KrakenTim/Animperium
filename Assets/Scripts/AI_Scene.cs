using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Centralises Scene names, to more easily find scene references and update scene names
/// </summary>
public static class AI_Scene
{
    public const string SCENENAME_Game = "NormanMapGeneration";
    public const string SCENENAME_GamePreparation = "GamePreparation";
    public const string SCENENAME_MainMenu = "MainMenu";
    public const string SCENENAME_MapEditor = "MapGeneration";
    public const string SCENENAME_OnlineRoom = "ServerClientChat";


    public static void LoadScene(string scenenName)
    {
        SceneManager.LoadScene(scenenName);
    }

    public static void LoadSceneWithLoadingScreen(string sceneName)
    {
        if (LoadingScreen.instance)
            LoadingScreen.instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}
