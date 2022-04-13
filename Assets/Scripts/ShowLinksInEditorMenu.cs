using UnityEngine;

#if UNITY_EDITOR
public class ShowLinksInEditorMenu
{
    const string LINK_FolderAnimperium = "https://drive.google.com/drive/folders/1e7PfbQ4q9hplT6IxhW3Qr6Vlc4S5dLYx";
    const string LINK_FolderGameDesign = "https://drive.google.com/drive/folders/1PRE40vgMsO-OMrDnZzxpHptMT8_tJKJx";

    const string LINK_Trello = "https://trello.com/invite/b/4XXhQrMb/70215122388291f678421b6e2cdcdd24/animperium";
    const string LINK_Miro = "https://miro.com/welcomeonboard/cjYxbURoYWJoU0VEdFBscEhsS0NmM0V3c3d3ZFVuN3E4QTRVQUNySGJQWTRJam5RMjZvRVZvaTZSYjBVTG9vZnwzNDU4NzY0NTIyNTQwODE2MzE0?share_link_id=899548609801";

    const string PATH_linkMenu = "Links/";

    // Folder

    [UnityEditor.MenuItem(PATH_linkMenu + "Animperium Folder")]
    public static void OpenFolderAnimperium() => Application.OpenURL(LINK_FolderAnimperium);

    [UnityEditor.MenuItem(PATH_linkMenu + "Game Design Folder")]
    public static void OpenFolderGameDesign() => Application.OpenURL(LINK_FolderGameDesign);

    // Tools

    [UnityEditor.MenuItem(PATH_linkMenu + "Trello")]
    public static void OpenTrello() => Application.OpenURL(LINK_Trello);

    [UnityEditor.MenuItem(PATH_linkMenu + "Miro")]
    public static void OpenMiro() => Application.OpenURL(LINK_Miro);
}
#endif