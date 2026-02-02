using Mirror;
using TMPro;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using Mirror.FizzySteam;
using UnityEngine.Rendering;
using System;

public class NetworkController : Singleton<NetworkController>
{
    private NetworkManager manager;
    private FizzyFacepunch fizzy;
    private const string HOST_ADDRESS = "hostAddress";
    private Lobby? currentLobby;

    protected override void Awake() {
        base.Awake();
    }

    private void Start() {
        manager = NetworkManager.singleton;

        if (manager == null) {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "NetworkManager is null");
            return;
        }
        
        if (manager.playerPrefab == null) {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "playerPrefab is null!");
            return;
        }

        if (!manager.TryGetComponent(out fizzy)) {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "failed to get fizzy...");
            return;
        }

        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested += OnLobbyJoinRequested;
    }

    private void OnDestroy() {
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= OnLobbyJoinRequested;

        if (SteamClient.IsValid) {
            SteamClient.Shutdown();
        }
    }

    private async void CreateLobby(int maxPlayer) {
        currentLobby = await SteamMatchmaking.CreateLobbyAsync(maxPlayer);

        if (!currentLobby.HasValue) {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "failed to create lobby...");
            return;
        }

        currentLobby?.SetFriendsOnly(); // refactor to switch
        currentLobby?.SetData("GameKey", "HideAndSlashJo");
        currentLobby?.SetJoinable(true);

        currentLobby?.SetData(HOST_ADDRESS, SteamClient.SteamId.ToString());
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        lobby.Refresh();
        string lobbyName = lobby.GetData("GameKey");
        Debug.Log(lobbyName);
        if (lobbyName != "HideAndSlashJo")
        {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "Wrong Lobby. Leaving...");
            lobby.Leave();
            return;
        }

        string hostSteamId = lobby.GetData(HOST_ADDRESS);

        fizzy.SteamUserID = SteamClient.SteamId;

        if (hostSteamId == SteamClient.SteamId.ToString())
        {
            manager.StartHost();
        }
        else
        {
            manager.networkAddress = hostSteamId;
            manager.StartClient();
        }
    }

    private async void OnLobbyJoinRequested(Lobby lobby, SteamId steamId) {
        RoomEnter enter = await lobby.Join();
        if (enter != RoomEnter.Success)
        {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "Failed to join lobby");
        }
    }

    public void OnHostButtonClicked() {
        CreateLobby(4);

        GameManager.Instance.EnterLockState(LockState.LOCKED);

        UIUtils.PrintUI(DEBUG_TYPE.ALERT, "Create Lobby Invoked");
    }

    public void OnClientButtonClicked() { //when client jump in to the lobby by entering lobbyId.
        UIUtils.PrintUI(DEBUG_TYPE.ALERT, "Entering the lobby...");
    }
}
