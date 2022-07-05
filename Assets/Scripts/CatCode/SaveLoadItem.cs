using UnityEngine;
using UnityEngine.UI;

public class SaveLoadItem : MonoBehaviour
{
    public SaveLoadMenu menu;

    #region not in tutorial
    public string mapPath;
    public MapSelectionMenu selectionMenu;
    #endregion not in tutorial

    public string MapName
    {
        get
        {
            return mapName;
        }
        set
        {
            mapName = value;
            transform.GetChild(0).GetComponent<Text>().text = value;
        }
    }

    string mapName;

    public void Select()
    {
        #region not in tutorial
        if (selectionMenu)
        {
            selectionMenu.SelectItem(this);
            return;
        }
        #endregion not in tutorial

        menu.SelectItem(mapName);
    }
}