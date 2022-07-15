using UnityEngine;

public class NewMapMenu : MonoBehaviour
{
    [Header("Muss Durch 5 Teilbar sein!")]
    public int SmallLength = 20;
    public int SmallHeight = 15;
    public int MediumLength = 40;
    public int MediumHeight = 30;
    public int LargeLength = 80;
    public int LargeHeight = 60;

    public HexGrid hexGrid;
    public void Open()
    {
        gameObject.SetActive(true);
        HexMapCamera.Locked = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HexMapCamera.Locked = false;
    }
    void CreateMap(int x, int z)
    {
        hexGrid.CreateMap(x, z);
        HexMapCamera.ValidatePosition();
        Close();
    }

    public void CreateSmallMap()
    {
        CreateMap(SmallLength, SmallHeight);
    }

    public void CreateMediumMap()
    {
        CreateMap(MediumLength, MediumHeight);
    }

    public void CreateLargeMap()
    {
        CreateMap(LargeLength, LargeHeight);
    }

}