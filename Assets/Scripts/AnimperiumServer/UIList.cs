using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIList : MonoBehaviour
{
    [SerializeField] protected GameObject ListItemPrefab;

    private ScrollRect scrollRectangle;

    private void Awake()
    {
        scrollRectangle = GetComponentInParent<ScrollRect>();
    }

    public virtual void AddItem(string _item)
    {
        GameObject newItem = Instantiate(ListItemPrefab, this.transform);
        newItem.GetComponent<TextMeshProUGUI>().text = _item;

        StartCoroutine(ScrollDown());
    }

    public void RefreshList(string _itemList)
    {
        Clear();

        string[] items = _itemList.Split('|');

        for (int i = 0; i < items.Length; i++)
        {
            AddItem(items[i]);
        }
    }

    public void Clear()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    private IEnumerator ScrollDown()
    {
        yield return new WaitForEndOfFrame();
        
        if (scrollRectangle)
            scrollRectangle.verticalNormalizedPosition = 0f;
    }
}
