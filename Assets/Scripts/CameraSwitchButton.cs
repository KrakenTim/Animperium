using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitchButton : MonoBehaviour
{
    public void Button_SwitchLayer()
    {
        HexMapCamera.SwapUsedGrid();
    }
}
