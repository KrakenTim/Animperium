using UnityEngine;

public class PlayerColorScript : MonoBehaviour
{
    public Color ChoosenColor;
    public Color[] PlayerColor;
    private int PlayerID = GameManager.ActivePlayerID;
    private Material PlayerColorMaterial;

    public void ChooseColor()
    {
        ChoosenColor = PlayerColor[PlayerID];
    }

    public void ChangePlayerColor()
    {
        PlayerColorMaterial.SetColor("_PlayerColor", PlayerColor[PlayerID]);    
    }
}
