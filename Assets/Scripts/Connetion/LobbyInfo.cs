using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfo : MonoBehaviour
{
    [SerializeField] private string m_lobbyName;
    [SerializeField] private string m_lobbyId;
    [SerializeField] private int m_nowPlayer;
    [SerializeField] private int m_maxPlayer;

    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text playerNum;
    [SerializeField] private Button joinButton;

    private void Awake() {
        joinButton.onClick.AddListener(OnJoinClicked);
    }

    public void SetLobbyData(string lobbyName, string lobbyId, int nowPlayer, int maxPlayer) {
        m_lobbyName = lobbyName;
        m_lobbyId = lobbyId;
        m_nowPlayer = nowPlayer;
        m_maxPlayer = maxPlayer;

        DisplayLobbyInfo();
    }

    public void DisplayLobbyInfo() {
        lobbyName.text = m_lobbyName;
        playerNum.text = $"{m_nowPlayer} / {m_maxPlayer}";
    }

    private void OnJoinClicked() {
        Debug.Log("Join Button Clicked");
        NetworkController.Instance.RequestJoin(m_lobbyId);
    }
}
