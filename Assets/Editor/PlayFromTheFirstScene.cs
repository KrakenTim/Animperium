using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// This class extends the edit drop down to allow users to start from the game or current scene
/// <summary>
public static class PlayFromTheFirstScene
{
    private const int startScreenScene = 0;

    private const string playFromStartScreenSceneMenuStr = "Edit/Always Start From Main Menu";

    private static bool playFromStartScreenScene
    {
        get
        {
            return EditorPrefs.HasKey(playFromStartScreenSceneMenuStr) &&
                   EditorPrefs.GetBool(playFromStartScreenSceneMenuStr);
        }
        set { EditorPrefs.SetBool(playFromStartScreenSceneMenuStr, value); }
    }

    [MenuItem(playFromStartScreenSceneMenuStr, false, 150)]
    private static void PlayFromStartScreenSceneCheckMenu()
    {
        playFromStartScreenScene = !playFromStartScreenScene;

        Menu.SetChecked(playFromStartScreenSceneMenuStr, playFromStartScreenScene);

        ShowNotifyOrLog(playFromStartScreenScene ? "Play from Main Menu scene" : "Play from current scene");
    }

    // The menu won't be gray out, we use this validate method for update check state
    [MenuItem(playFromStartScreenSceneMenuStr, true)]
    static bool PlayFromStartScreenSceneCheckMenuValidate()
    {
        Menu.SetChecked(playFromStartScreenSceneMenuStr, playFromStartScreenScene);
        return true;
    }

    // This method is called before any Awake. It's the perfect callback for this feature
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void LoadFirstSceneAtGameBegins()
    {
        if (!playFromStartScreenScene) return;

        if (EditorBuildSettings.scenes.Length <= startScreenScene)
        {
            Debug.LogWarning($"Editor\tThe scene build list is empty. Can't play from" +
                             $" {(playFromStartScreenScene ? "the start menu" : "")} scene.\n");
            return;
        }

        foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
            go.SetActive(false);

        SceneManager.LoadScene(startScreenScene);
    }

    static void ShowNotifyOrLog(string msg)
    {
        if (Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
            EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent(msg));
        else
            Debug.Log("Editor\t" + msg + "\n"); // When there's no scene view opened, we just print a log
    }
}