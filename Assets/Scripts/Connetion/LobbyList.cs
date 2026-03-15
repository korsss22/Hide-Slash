using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private LobbyInfo lobbyPanel;
    private List<LobbyInfo> lobbyList = new();
    private bool isLoading = false;

    private void OnEnable() {
        NetworkController.Instance.OnRequestLobby += SetLobbyList;
        RefreshLobbyList();
    }

    private void OnDisable()
    {
        NetworkController.Instance.OnRequestLobby -= SetLobbyList;
    }

    private async void SetLobbyList() {
        if (isLoading) return;
        
        try {
            isLoading = true;

            Lobby[] lobbies = await NetworkController.Instance.GetLobbyList();

            foreach (var lobby in lobbies)
        {
            string lobbyName = lobby.GetData("LobbyName");
            string lobbyId = lobby.Id.ToString();
            int nowPlayer = lobby.MemberCount;
            int.TryParse(lobby.GetData("MaxPlayer"), out int maxPlayer);

            LobbyInfo lobbyInfo = Instantiate(lobbyPanel, transform);
            lobbyInfo.SetLobbyData(lobbyName, lobbyId, nowPlayer, maxPlayer);

            lobbyList.Add(lobbyInfo);
        }
        } catch(Exception e) {
            UIManager.PrintUI(DEBUG_TYPE.ERROR, "faild to get LobbyData. : "+e.Message);
        } finally {
            isLoading = false;
        }
    }

    public void RefreshLobbyList() {
        if (isLoading) return;

        foreach (var lobby in lobbyList)
        {
            Destroy(lobby.gameObject);
        }

        lobbyList.Clear(); 

        SetLobbyList();
    }
}
