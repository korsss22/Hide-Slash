using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private GameObject lobbyList;
    [SerializeField] private GameObject lobbySetting;
    [SerializeField] private GameObject lobby;

    private GameObject nowScreen = null;

    private void Start() {
    }

    private void OnDisable() {
    }

    public void LobbyList() {
        SwitchScreen(lobbyList);
    }

    public void LobbySetting() {
        SwitchScreen(lobbySetting);
    }

    public void Lobby() {
        SwitchScreen(lobby);
    }

    private void SwitchScreen(GameObject targetScreen) {
        if (nowScreen != null) nowScreen.SetActive(false);

        nowScreen = targetScreen;
        nowScreen.SetActive(true);
    }
}
