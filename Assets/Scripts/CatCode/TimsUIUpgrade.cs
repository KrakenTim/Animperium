using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimsUIUpgrade : MonoBehaviour
{
    public GameObject LandPlacement;

    public GameObject WaterPlacement;

    public GameObject DecoPlacement;

    public HexMapEditor HexEditor;

    private void Awake()
    {
        HexEditor.SetApplyWaterLevel(false);
        HexEditor.SetApplyDecoLevel(false);
        LandPlacement.SetActive(true);
        WaterPlacement.SetActive(false);
        DecoPlacement.SetActive(false);
    }

    public void ShowLand()
    {
        HexEditor.SetApplyWaterLevel(false);
        HexEditor.SetApplyDecoLevel(false);
        HexEditor.SetApplyPlantLevel(false);
        HexEditor.SetApplySpecialIndex(false);
        LandPlacement.SetActive(true);
        WaterPlacement.SetActive(false);
        DecoPlacement.SetActive(false);
    }

    public void ShowWater()
    {
        HexEditor.SetTerrainTypeIndex(-1);
        HexEditor.SetApplyElevation(false);
        HexEditor.SetApplyWaterLevel(true);
        HexEditor.SetApplyDecoLevel(false);
        HexEditor.SetApplyPlantLevel(false);
        HexEditor.SetApplySpecialIndex(false);
        LandPlacement.SetActive(false);
        WaterPlacement.SetActive(true);
        DecoPlacement.SetActive(false);

    }

    public void ShowDeco()
    {
        HexEditor.SetTerrainTypeIndex(-1);
        HexEditor.SetApplyElevation(false);
        HexEditor.SetApplyWaterLevel(false);
        HexEditor.SetApplyDecoLevel(true);
        HexEditor.SetApplyPlantLevel(false);
        HexEditor.SetApplySpecialIndex(false);
        LandPlacement.SetActive(false);
        WaterPlacement.SetActive(false);
        DecoPlacement.SetActive(true);

    }
}
