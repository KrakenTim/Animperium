using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerValueProvider : MonoBehaviour
{
    [SerializeField] PlayerValues[] playerValues;

    Dictionary<int, Transform> spawnFolderTransforms = new Dictionary<int, Transform>();

    /// <summary>
    /// Return true if the given player belongs to a diffrent faction than the local player.
    /// </summary>
    public bool isEnemy(int otherPlayerID)
    {
        if (TryGetPlayerValues(GameManager.LocalPlayerID, out PlayerValues currentPlayer)
            && TryGetPlayerValues(otherPlayerID, out PlayerValues otherPlayer))
        {
            return currentPlayer.factionID != otherPlayer.factionID;
        }
        return false;
    }

    /// <summary>
    /// Uses Names from server in online mode, sets default name for Player in hotseat if none is given.
    /// All players get their townhall as first camera position.
    /// </summary>
    public void SetupPlayerStart()
    {
        if (OnlineGameManager.IsOnlineGame)
            OnlineGameManager.SetupPlayerNames(playerValues);
        else
            SetDefaultNamesForMissing();

        foreach (var player in playerValues)
            player.lastCameraValues.localPosition = player.GetTownHall().WorldPosition;
    }

    /// <summary>
    /// Adds PlayerIDs and connected Transform to spawnFolderTransforms.
    /// Renames spawn list transforms to player names.
    /// Creates additional sorting transforms below the game manager if needed.
    /// </summary>
    public void SetupPawnFolders()
    {
        while (playerValues.Length >= transform.childCount)
        {
            var s = new GameObject();
            s.transform.parent = transform;
        }

        for (int i = 1; i <= playerValues.Length; i++)
        {
            spawnFolderTransforms.Add(playerValues[i - 1].playerID, transform.GetChild(i));
            transform.GetChild(i).gameObject.name = playerValues[i - 1].name;
        }
    }

    /// <summary>
    /// Returns the transform parent for a player's pawns.
    /// Uses own transform as fallback, if no transform is given for the player.
    /// </summary>
    public Transform GetPlayerPawnParrent(int playerID)
    {
        if (spawnFolderTransforms.ContainsKey(playerID))
            return spawnFolderTransforms[playerID];
        else
        {
            Debug.LogError($"PlayerValueProvider\tpawn Transform for unknown playerID requested: {playerID}\n", this);
            return transform;
        }
    }

    /// <summary>
    /// Tries to get the PlayerValues of the given playerID, returns true if successfull.
    /// </summary>
    public bool TryGetPlayerValues(int playerID, out PlayerValues result)
    {
        foreach (var item in playerValues)
        {
            if (item.playerID == playerID)
            {
                result = item;
                return true;
            }

        }

        Debug.LogError("Values not found for Player " + playerID, this);

        result = new PlayerValues();

        return false;
    }

    /// <summary>
    /// Return the playerID of the next existing player that didn't lose already.
    /// </summary>
    public int NextActivePlayer(int playerID)
    {
        int resultPlayerID = playerID + 1;

        if (resultPlayerID > playerValues.Length)
            resultPlayerID = 1;

        if (TryGetPlayerValues(resultPlayerID, out PlayerValues nextPlayer)
            && !nextPlayer.HasLost)
        {
            return resultPlayerID;
        }

        // player either didn't exist has lost already
        return NextActivePlayer(resultPlayerID);
    }

    /// <summary>
    /// set missing names to default
    /// </summary>
    public void SetDefaultNamesForMissing()
    {
        for (int i = 0; i < playerValues.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(playerValues[i].name))
                playerValues[i].name = playerValues[i].DefaultName;
        }
    }

    /// <summary>
    /// True if the game ends because only one faction is left.
    /// winners are rhe members of the remaining faction (also those wich lost); 
    /// </summary>
    public bool CheckIfGameEnds(int potencialLoserPlayerID, out List<PlayerValues> winners)
    {
        winners = new List<PlayerValues>();

        if (!TryGetPlayerValues(potencialLoserPlayerID, out PlayerValues loserValues))
            return false;

        if (!loserValues.CheckIfHasLost()) return false;

        // int: fractionId, List<PlayerValues>: surviving Players
        HashSet<int> remainingFactionIDs = new HashSet<int>();

        foreach (PlayerValues player in playerValues)
        {
            if (player.HasLost) continue;

            remainingFactionIDs.Add(player.factionID);
        }

        if (remainingFactionIDs.Count > 1) return false;

        foreach (var player in playerValues)
        {
            if (remainingFactionIDs.Contains(player.factionID))
                winners.Add(player);
        }
        return true;
    }
}
