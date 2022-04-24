using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRoomList : UIList
{
    public override void AddItem(string _roomName)
    {
        Button newItem = Instantiate(ListItemPrefab, this.transform).GetComponent<Button>();
        newItem.GetComponentInChildren<TextMeshProUGUI>().text = _roomName;
        newItem.onClick.AddListener(delegate { ServerConnection.Instance.JoinRoom(_roomName); });
    }
}
