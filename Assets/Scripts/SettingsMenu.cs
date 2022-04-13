using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{
    // Start is called before the first frame update
  
    public void SetVolume (float volume)
    {
        Debug.Log(volume);
    }

    List<int> widths = new List<int>() { 1280, 1920, 2560, 3840};
    List<int> heights = new List<int>() { 720, 800, 1024, 1080, 1440, 2160};

    public void SetScreenSize(int index)

    {
        bool fullscreen = Screen.fullScreen;
        int width = widths[index];
        int height = heights[index];
        Screen.SetResolution(width, height, fullscreen);
    }

    public void SetFullscreen(bool isfullscreen)

    {
        Screen.fullScreen = isfullscreen;
    }
}
