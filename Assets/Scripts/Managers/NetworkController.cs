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

public class NetworkController : Singleton<NetworkController>
{
    private NetworkManager manager;
    private FizzyFacepunch fizzy;
    private const string HOST_ADDRESS = "hostAddress";
    private Lobby? currentLobby;
    [SerializeField] private TransportType transportType;
    private Action onStartHost;
    private Action<string> onStartClient;
    public Action OnRequestLobby;

    protected override void Awake() {
        base.Awake();

        Action hostCallback = transportType == TransportType.STEAM ? OnHostButtonClickedWithSteam : OnHostButtonClickedWithTCP;
        Action<string> clientCallback = transportType == TransportType.STEAM ? OnClientButtonClickedWithSteam : OnClientButtonClickedWithTCP;

        onStartHost += hostCallback;
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
        if (onStartHost == null) {
            Debug.Log("Host Action is null..");
            return;
        }

        onStartHost.Invoke();
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

    private void OnHostButtonClickedWithSteam() {
        CreateLobby(4);

        GameManager.Instance.EnterLockState(LockState.LOCKED);
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

    private async void CreateLobby(int maxPlayer) {
        currentLobby = await SteamMatchmaking.CreateLobbyAsync(maxPlayer);


        if (!currentLobby.HasValue) {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "failed to create lobby...");
            return;
        }

        currentLobby?.SetPublic(); // refactor to switch

        currentLobby?.SetJoinable(true);

        //It's for test. refactor SaveDatas to Variable.
        currentLobby?.SetData("LobbyName", "MyLobby");
        currentLobby?.SetData("MaxPlayer", "16");
        currentLobby?.SetData(HOST_ADDRESS, SteamClient.SteamId.ToString());
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        lobby.Refresh();
        
    }

    private async void OnLobbyJoinRequested(Lobby lobby, SteamId steamId) {
        RoomEnter enter = await lobby.Join();

        if (enter != RoomEnter.Success)
        {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "Failed to join lobby");
        }
    }
}
