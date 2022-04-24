using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIList : MonoBehaviour
{
    [SerializeField] protected GameObject ListItemPrefab;

    public virtual void AddItem(string _item)
    {
        GameObject newItem = Instantiate(ListItemPrefab, this.transform);
        newItem.GetComponent<TextMeshProUGUI>().text = _item;
    }

    public void RefreshList(string _itemList)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        string [] items = _itemList.Split('|');

        for (int i = 0; i < items.Length; i++)
        {
            AddItem(items[i]);
        }
    }
}
