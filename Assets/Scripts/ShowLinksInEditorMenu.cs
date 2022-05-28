#if UNITY_EDITOR // whole file
using UnityEditor;
using UnityEngine;

/// <summary>
/// Creates menu options to open several permanent links related to the project. 
/// </summary>
public class ShowLinksInEditorMenu
{
    const string LINK_FolderAnimperium = "https://drive.google.com/drive/folders/1e7PfbQ4q9hplT6IxhW3Qr6Vlc4S5dLYx";
    const string LINK_FolderGameDesign = "https://drive.google.com/drive/folders/1PRE40vgMsO-OMrDnZzxpHptMT8_tJKJx";

    const string Link_Codecks = "https://ga-gd-1.codecks.io";
    const string LINK_Miro = "https://miro.com/welcomeonboard/cjYxbURoYWJoU0VEdFBscEhsS0NmM0V3c3d3ZFVuN3E4QTRVQUNySGJQWTRJam5RMjZvRVZvaTZSYjBVTG9vZnwzNDU4NzY0NTIyNTQwODE2MzE0?share_link_id=899548609801";
    const string LINK_Trello = "https://trello.com/invite/b/4XXhQrMb/70215122388291f678421b6e2cdcdd24/animperium";

    const string LINK_PawnBalancingTable = "https://docs.google.com/spreadsheets/d/13qi9Cj-XVV9LdGZXMuxDs1aicuXQf038dLEMzz_f5Nk";
    const string LINK_LocalisationTable = "https://docs.google.com/spreadsheets/d/1bF9-KAq6S5hRU3yXoR4UtxqKVag8gwAsi8phnBVAtiA";

    const string PATH_linkMenu = "Links/";

    // Folder

    [MenuItem(PATH_linkMenu + "Animperium Folder", false, 1)]
    public static void OpenFolderAnimperium() => Application.OpenURL(LINK_FolderAnimperium);

    [MenuItem(PATH_linkMenu + "Game Design Folder", false, 1)]
    public static void OpenFolderGameDesign() => Application.OpenURL(LINK_FolderGameDesign);

    // Coordination

    [MenuItem(PATH_linkMenu + "Codecks", false, 100)]
    public static void OpenCodecks() => Application.OpenURL(Link_Codecks);

    [MenuItem(PATH_linkMenu + "Miro", false, 100)]
    public static void OpenMiro() => Application.OpenURL(LINK_Miro);

    [MenuItem(PATH_linkMenu + "Trello", false, 100)]
    public static void OpenTrello() => Application.OpenURL(LINK_Trello);

    // Tables

    [MenuItem(PATH_linkMenu + "Balancing Table", false, 200)]
    public static void OpenBalanceTable() => Application.OpenURL(LINK_PawnBalancingTable);

    [MenuItem(PATH_linkMenu + "Localisation Table", false, 200)]
    public static void OpenLocalisationTable() => Application.OpenURL(LINK_LocalisationTable);
}
#endif // whole file