using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : MonoBehaviour
{
    [SerializeField] int playerID;
    [SerializeField] int fractionID;

    [SerializeField] ePlayerPawnType type;

    [SerializeField] HexCell hexCell;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOwner(int playerID, int fractionID)
    {
        this.playerID = playerID;
        this.fractionID = fractionID;
    }

    public void SetHexCell(HexCell cell)
    {
        hexCell = cell;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (hexCell)
            transform.position = hexCell.transform.position;
        else
            Debug.LogError("Tried to Update Position without HexCell", this);
    }
}
