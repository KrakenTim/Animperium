using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides methods to change different options in the settings menu.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [SerializeField] Slider VolumeSlider;

    List<int> widths = new List<int>() { 1280, 1280, 1280, 1920, 2560, 3840};
    List<int> heights = new List<int>() { 720, 800, 1024, 1080, 1440, 2160};

    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = VolumeSlider.value;
        Save();
    }

    private void Load()
    {
        VolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", VolumeSlider.value);
    }
    //public void SetVolume (float volume)
    //{
    //    Debug.Log(volume);
    //}

    public void SetScreenSize(int index)
    {
        bool fullscreen = Screen.fullScreen;
        int width = widths[index];
        int height = heights[index];
        Screen.SetResolution(width, height, fullscreen);

        Debug.Log("ScreenSize is" + width + height);
    }

    public void SetFullscreen(bool isfullscreen)
    {
        Screen.fullScreen = isfullscreen;
        Debug.Log("Fullscreen is" + isfullscreen);
    }
}
