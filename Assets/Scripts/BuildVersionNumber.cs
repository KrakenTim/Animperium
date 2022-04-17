using System;
using System.IO;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

#if UNITY_EDITOR
public class BuildVersionNumber : MonoBehaviour, IPreprocessBuildWithReport
#else
public class BuildVersionNumber : MonoBehaviour
#endif
{
    #region Variables

    public static string Path_VersionFile => Application.streamingAssetsPath + "/version.txt";

    const bool RemovePlayerPrefsBeforeBuild = true;

    [SerializeField] bool dateWithoutNumber = false;

    bool defaultVisible = true;
    bool IsVisible => deactivatedIfInvisible ? deactivatedIfInvisible.activeSelf : true;

    private string versionText;

#pragma warning disable CS0649 // never assigned
    [SerializeField] TMP_Text myText;
    [SerializeField] GameObject deactivatedIfInvisible;
#pragma warning restore CS0649 // never assigned

    #endregion Variables

    private void OnEnable()
    {
        if (versionText == null)
            UpdateVersionText();

        Show(defaultVisible);

        DontDestroyOnLoad(GetComponentInParent<Canvas>().gameObject);
    }

    public void Show(bool isVisible)
    {
        if (deactivatedIfInvisible)
            deactivatedIfInvisible.SetActive(isVisible);
    }

    public void UpdateVersionText()
    {
        versionText = null;
        if (File.Exists(Path_VersionFile))
        {
            string[] versionInfo = File.ReadAllLines(Path_VersionFile);
            versionText = dateWithoutNumber ?
                          versionInfo[0] : ("Version " + Application.version + " [" + versionInfo[2] + "]\n" + versionInfo[0]);
        }

        myText.text = (versionText != null) ? versionText : Application.productName + " " + Application.version;
    }

    public static void FlipVisibility()
    {
        BuildVersionNumber number = FindObjectOfType<BuildVersionNumber>();

        if (number && number.myText)
            number.Show(!number.IsVisible);
    }

    #region Update version.txt

#if UNITY_EDITOR
    public int callbackOrder => 0;
    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_PRO_LICENSE
        if (PlayerSettings.SplashScreen.show)
            PlayerSettings.SplashScreen.show = false;
#endif
        if (RemovePlayerPrefsBeforeBuild) PlayerPrefs.DeleteAll();

        string[] versionInfo;
        int buildNumber;

        if (File.Exists(Path_VersionFile))
            versionInfo = File.ReadAllLines(Path_VersionFile);
        else
            versionInfo = new string[3];

        versionInfo[0] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        versionInfo[1] = Application.version;
        versionInfo[2] = int.TryParse(versionInfo[2], out buildNumber) ? (buildNumber + 1).ToString() : "1";

        Debug.Log("Build\t\t" + Application.version + " [" + versionInfo[2] + "]\n\t\t" + versionInfo[0]);

        if (!Directory.Exists(Path.GetDirectoryName(Path_VersionFile)))
            Directory.CreateDirectory(Path.GetDirectoryName(Path_VersionFile));

        File.WriteAllLines(Path_VersionFile, versionInfo);
    }
#endif
    #endregion Update version.txt
}