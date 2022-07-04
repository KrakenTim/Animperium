using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningManager : MonoBehaviour
{
    public GameObject LightingUnderground;
    public GameObject LightingDay;

    private void Start()
    {
        HexMapCamera.Instance.OnSwapToGrid.AddListener(SwitchLighting); 
       
    }

    public void SwitchLighting(HexGridLayer hexGridLayer)
    {
        LightingUnderground.SetActive(hexGridLayer == HexGridLayer.Underground);
        LightingDay.SetActive(hexGridLayer == HexGridLayer.Surface);
    }
}
