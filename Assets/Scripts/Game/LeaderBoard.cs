﻿using UnityEngine;
using UnityEngine.Networking;

/// <summary>
///     The LeaderBoard which is shown at the end of a game.
///     Contains player ranks, player names and player survival times.
/// </summary>
public class LeaderBoard : NetworkBehaviour {

    //================================================================================
    // Prefab components (Inspector)
    //================================================================================

    [Header("UI References")]
    [Tooltip("Player List Reference")]
    public RectTransform playerList;

    [Tooltip("Quit Button")]
    public RectTransform quitButton;

    [Tooltip("Restart Button")]
    public RectTransform restartButton;

    [Tooltip("Waiting Button")]
    public RectTransform waitingButton;

    [Tooltip("List Entry Prefab")]
    public Transform listEntryPrefab;

    [Tooltip("Leaderboard Canvas")]
    public Canvas canvas;

    //================================================================================
    // Server properties
    //================================================================================

    public int playersReady = 0;

    //================================================================================
    // Logic
    //================================================================================

    /// <summary>
    ///     Adds a player to the Leaderboard.
    /// </summary>
    /// <param name="player">GamePlayer to add</param>
    public void AddPlayer(GamePlayer player) {
        Transform listEntryTransform = Instantiate(listEntryPrefab);
        ListEntry listEntry = listEntryTransform.GetComponent<ListEntry>();
        listEntry.setPlayerName(player.playerName);
        listEntry.setPlayerTime(player.getLifeTime());
        listEntryTransform.SetParent(playerList);
        // Add as first element inside visible list
        listEntryTransform.SetAsFirstSibling();

        // listEntryTransform gets instantiated with size (100,100,100) although prefab has size of (1,1,1) ???!!!!!
        listEntryTransform.localScale = new Vector3(1f, 1f, 1f);
        listEntryTransform.localPosition = listEntryTransform.localPosition - new Vector3(0, 0, listEntryTransform.localPosition.z);

        ListEntry[] listEntries = playerList.GetComponentsInChildren<ListEntry>();
        int tableEntries = listEntries.Length;
        for(int i = 0; i < tableEntries; i++) {
            listEntries[i].setRank(i + 1);
        }
    }

    public void OnBackToMenuClicked() {
        CustomLobbyManager.lobbyManagerSingleton.GoBackButton();
    }

    public void OnRestartClicked() {
        quitButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        waitingButton.gameObject.SetActive(true);

        NetworkClient.allClients[0].connection.playerControllers[0].gameObject.GetComponent<GamePlayer>().CmdPlayerReadyToRestart();
    }

    public void OnWaitingClicked() {
        quitButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        waitingButton.gameObject.SetActive(false);

        NetworkClient.allClients[0].connection.playerControllers[0].gameObject.GetComponent<GamePlayer>().CmdPlayerNotReadyToRestart();
    }

    public void Reset() {
        int children = playerList.childCount;

        for(int i = 0; i < children; i++) {
            Destroy(playerList.GetChild(i).gameObject);
        }

        quitButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        waitingButton.gameObject.SetActive(false);
    }

    /// <summary>
    ///     Hides/Shows the LeaderBoard.
    /// </summary>
    /// <param name="enabled">true: show LeaderBoard - false: hide LeaderBoard</param>
    public void ToggleVisibility(bool enabled) {
        canvas.enabled = enabled;
    }

}
