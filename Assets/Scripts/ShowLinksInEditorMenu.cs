using UnityEngine;

#if UNITY_EDITOR
public class ShowLinksInEditorMenu
{
    const string LINK_FolderAnimperium = "https://drive.google.com/drive/folders/1e7PfbQ4q9hplT6IxhW3Qr6Vlc4S5dLYx";
    const string LINK_FolderGameDesign = "https://drive.google.com/drive/folders/1PRE40vgMsO-OMrDnZzxpHptMT8_tJKJx";

    const string PATH_linkMenu = "Links(Google)/";

    [UnityEditor.MenuItem(PATH_linkMenu + "Animperium Folder")]
    public static void OpenFolderAnimperium() => Application.OpenURL(LINK_FolderAnimperium);

    [UnityEditor.MenuItem(PATH_linkMenu + "Game Design Folder")]
    public static void OpenFolderGameDesign() => Application.OpenURL(LINK_FolderGameDesign);
}
#endif