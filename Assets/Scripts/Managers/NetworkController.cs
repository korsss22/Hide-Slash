using Mirror;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using Mirror.FizzySteam;
using System;
using System.Threading.Tasks;

public enum TransportType {
    STEAM = 0,
    TCP
}

public enum LobbyType {
    Public = 0,
    FriendsOnly,
    InviteOnly
}

public class NetworkController : Singleton<NetworkController>
{
    private NetworkManager manager;
    private FizzyFacepunch fizzy;
    private const string HOST_ADDRESS = "hostAddress";
    private Lobby? currentLobby;
    [SerializeField] private TransportType transportType;
    private Action<string> onStartClient;
    public Action OnRequestLobby;
    public Action OnLobbyEnter;

    protected override void Awake() {
        base.Awake();

        Action<string> clientCallback = transportType == TransportType.STEAM ? OnClientButtonClickedWithSteam : OnClientButtonClickedWithTCP;

        onStartClient += clientCallback;
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

    [ClientCallback]
    private void OnDestroy() {
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= OnLobbyJoinRequested;
    }


    public void OnStartButtonClicked() {

    }

    public void OnJoinButtonClicked() {
        OnRequestLobby?.Invoke();
    }

    public async Task<Lobby[]> GetLobbyList() {
        var result = await SteamMatchmaking.LobbyList.RequestAsync();
        return result ?? Array.Empty<Lobby>();
    }
    
    public void RequestJoin(string lobbyId) {
        if (onStartClient == null) {
            Debug.Log("Client Action is null..");
            return;
        }

        onStartClient.Invoke(lobbyId);
    }

    private async void OnClientButtonClickedWithSteam(string lobbyId) { //when client jump into the lobby by entering lobbyId.
        UIUtils.PrintUI(DEBUG_TYPE.ALERT, "Entering the lobby...");

        if (!ulong.TryParse(lobbyId, out ulong id)) return;

        Lobby lobby = new(id);

        RoomEnter flag = await lobby.Join();
        
        if (flag != RoomEnter.Success) {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "failed to join lobby...");
        }
    }

    private void OnApplicationQuit() {
        if (SteamClient.IsValid) {
            SteamClient.Shutdown();
        }
    }

    private void OnHostButtonClickedWithTCP() {
        manager.StartHost();
    }

    private void OnClientButtonClickedWithTCP(string _) {
        manager.networkAddress = "localhost";
        manager.StartClient();
    }

/*--------------------------SteamCallbacks----------------------------*/

    public async void CreateLobby(string lobbyName, string password, int maxPlayer, LobbyType lobbyType) {
        currentLobby = await SteamMatchmaking.CreateLobbyAsync(maxPlayer);

        if (!currentLobby.HasValue) {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "failed to create lobby...");
            return;
        }

        switch (lobbyType)
        {
            case LobbyType.Public:
                currentLobby?.SetPublic();
                break;
            case LobbyType.FriendsOnly:
                currentLobby?.SetFriendsOnly();
                break;
            case LobbyType.InviteOnly:
                currentLobby?.SetPrivate();
                break;
            default:
                currentLobby?.SetPublic();
                break;
        }

        currentLobby?.SetJoinable(true);

        currentLobby?.SetData("LobbyName", lobbyName);
        currentLobby?.SetData("Password", password);
        currentLobby?.SetData("MaxPlayer", maxPlayer.ToString());
        currentLobby?.SetData(HOST_ADDRESS, SteamClient.SteamId.ToString());
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        if (NetworkServer.active || NetworkClient.active)
        {
            Debug.Log("Network already active, ignore StartHost");
            return;
        }
        lobby.Refresh();
        OnLobbyEnter?.Invoke();
        
        string gameId = lobby.GetData(HOST_ADDRESS);

        if (gameId == null) return;

        if (gameId == SteamClient.SteamId.ToString()) { //host
            manager.StartHost();
        } else {
            manager.networkAddress = gameId;
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
}
