using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] Image background;
    [Space]
    [SerializeField] GameObject pawnInfoRoot;
    [SerializeField] TMPro.TMP_Text pawnType;
    [SerializeField] TMPro.TMP_Text pawnHP;
    [SerializeField] TMPro.TMP_Text pawnMP;

    private void Awake()
    {
        GameManager.TurnStarted += UpdateHUD;
        GameInputManager.SelectedPawn += UpdateSelectedUnit;
    }

    private void OnDestroy()
    {
        GameManager.TurnStarted -= UpdateHUD;
        GameInputManager.SelectedPawn -= UpdateSelectedUnit;
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

    private void UpdateSelectedUnit(PlayerPawn selectedPawn)
    {
        pawnInfoRoot.SetActive(selectedPawn != null);

        if (selectedPawn == null) return;


        pawnType.text = selectedPawn.PawnType.ToString();

        pawnHP.text = "HP: " + selectedPawn.HP;
        pawnMP.text = "MP: " + selectedPawn.MP;
        pawnMP.enabled = selectedPawn.IsUnit;
    }   

    public void Button_EndTurn()
    {
        GameManager.EndTurn();
    }

}
