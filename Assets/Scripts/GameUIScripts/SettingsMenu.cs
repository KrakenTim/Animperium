using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides methods to change different options in the settings menu.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    const string VOLOUME_Master = "MasterVolume";

    [SerializeField] Dropdown SelectLanguage;
    [SerializeField] Dropdown SelectResolution;
    [SerializeField] Slider VolumeSlider;

    List<int> widths = new List<int>() { 1280, 1280, 1280, 1920, 2560, 3840 };
    List<int> heights = new List<int>() { 720, 800, 1024, 1080, 1440, 2160 };
    List<eLanguage> languageList = new List<eLanguage>() { eLanguage.English, eLanguage.German};

    void Start()
    {
        SetVolumeToPreference();
        Load();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = VolumeSlider.value;
        Save();
    }

    public static void SetVolumeToPreference()
    {
        if (!PlayerPrefs.HasKey(VOLOUME_Master))
            PlayerPrefs.SetFloat(VOLOUME_Master, 1f);
            
        AudioListener.volume = PlayerPrefs.GetFloat(VOLOUME_Master);
    }

    private void Load()
    {
        VolumeSlider.value = PlayerPrefs.GetFloat(VOLOUME_Master);

        //for (int i = 0; i < widths.Count + heights.Count; i++)
        //{
        //    SelectResolution.value = i;
        //}

        for (int i = 0; i < languageList.Count; i++)
        {
            if (Localisation.Instance.CurrentLanguage == languageList[i])
                SelectLanguage.value = i;
        }
    }

    private void Save()
    {
        PlayerPrefs.SetFloat(VOLOUME_Master, VolumeSlider.value);
    }

    public void SetScreenSize(int index)
    {
        bool fullscreen = Screen.fullScreen;
        int width = widths[index];
        int height = heights[index];
        Screen.SetResolution(width, height, fullscreen);

        int Entry = SelectResolution.value;

        Debug.Log("ScreenSize is" + width + 'x' + height);
    }

    public void SetFullscreen(bool isfullscreen)
    {
        Screen.fullScreen = isfullscreen;
        Debug.Log("Fullscreen is" + isfullscreen);
    }

    public void GetLocalisation()
    {
        int Entry = SelectLanguage.value;

        Localisation.Instance.SetLanguage(languageList[Entry]);
        //Debug.Log("Play Game in English");
        //Localisation.Instance.SetLanguage(eLanguage.German);
        //Debug.Log("Play Game in German");
        //Localisation.Instance.SetLanguage(eLanguage.Polish);
        //Localisation.Instance.SetLanguage(eLanguage.Spanish);
        //Localisation.Instance.SetLanguage(eLanguage.Italian);
    }
}
