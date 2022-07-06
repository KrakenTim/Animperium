using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Removes relicts from previous games and the online mode
/// </summary>
public class PersistentObjectCleaner : MonoBehaviour
{
    private void OnEnable()
    {
        ServerConnection serverConnection = FindObjectOfType<ServerConnection>();

        if (serverConnection)
            Destroy(serverConnection.gameObject);
    }
}
