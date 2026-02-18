using UnityEngine;

public class LobbyScreen : MonoBehaviour
{
    void Start()
    {
        NetworkController.Instance.OnLobbyEnter += ActiveLobby;
    }

    private void OnDisable() {
        NetworkController.Instance.OnLobbyEnter -= ActiveLobby;
    }

    private void ActiveLobby() {
        Debug.Log("ActiveLobby");
        this.gameObject.SetActive(true);
    }
}
