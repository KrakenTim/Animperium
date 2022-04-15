using UnityEngine;

#if UNITY_EDITOR
public class ShowLinksInEditorMenu
{
    const string LINK_FolderAnimperium = "https://drive.google.com/drive/folders/1e7PfbQ4q9hplT6IxhW3Qr6Vlc4S5dLYx";
    const string LINK_FolderGameDesign = "https://drive.google.com/drive/folders/1PRE40vgMsO-OMrDnZzxpHptMT8_tJKJx";

    const string LINK_Trello = "https://trello.com/invite/b/4XXhQrMb/70215122388291f678421b6e2cdcdd24/animperium";
    const string LINK_Miro = "https://miro.com/welcomeonboard/cjYxbURoYWJoU0VEdFBscEhsS0NmM0V3c3d3ZFVuN3E4QTRVQUNySGJQWTRJam5RMjZvRVZvaTZSYjBVTG9vZnwzNDU4NzY0NTIyNTQwODE2MzE0?share_link_id=899548609801";

    const string LINK_PawnBalancingTable = "https://docs.google.com/spreadsheets/d/13qi9Cj-XVV9LdGZXMuxDs1aicuXQf038dLEMzz_f5Nk";

    const string PATH_linkMenu = "Links/";

    // Folder

    [UnityEditor.MenuItem(PATH_linkMenu + "Animperium Folder",false,1)]
    public static void OpenFolderAnimperium() => Application.OpenURL(LINK_FolderAnimperium);

    [UnityEditor.MenuItem(PATH_linkMenu + "Game Design Folder",false, 1)]
    public static void OpenFolderGameDesign() => Application.OpenURL(LINK_FolderGameDesign);
    
    // Others

    [UnityEditor.MenuItem(PATH_linkMenu + "Balancing Table",false, 100)]
    public static void OpenBalanceTable() => Application.OpenURL(LINK_PawnBalancingTable);

    [UnityEditor.MenuItem(PATH_linkMenu + "Miro", false, 100)]
    public static void OpenMiro() => Application.OpenURL(LINK_Miro);

    [UnityEditor.MenuItem(PATH_linkMenu + "Trello",false, 100)]
    public static void OpenTrello() => Application.OpenURL(LINK_Trello);    
}
#endif