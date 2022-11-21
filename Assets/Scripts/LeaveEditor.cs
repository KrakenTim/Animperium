using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Leaves the Map-Editor 
/// </summary>
public class LeaveEditor : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        AI_Scene.LoadScene(AI_Scene.SCENENAME_MainMenu);
    }
}
