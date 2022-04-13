using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] Image background;


    private void Awake()
    {
        GameManager.TurnStarted += UpdateHUD; 
    }

    private void OnDestroy()
    {
        GameManager.TurnStarted -= UpdateHUD;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateHUD(GameManager.ActivePlayerID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateHUD(int playerID)
    {
        background.color = GameManager.GetPlayerColor(playerID);
    }

    public void Button_EndTurn()
    {
        GameManager.EndTurn();
    }

}
