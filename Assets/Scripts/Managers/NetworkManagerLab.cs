using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetworkManagerLab : NetworkManager
{
    public override void OnStartClient() {
        base.OnStartClient();

        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "Map":
                GameManager.Instance.StartLobbyWaiting();
                break;
            case "MainScene":
                Debug.Log("leaved game...");
                break;
            default:
                break;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        base.OnServerAddPlayer(conn);
        Debug.Log("Add Player. update Wait UI. Player : "+conn.address);
        UIManager.Instance.UpdateWaitUI();
    }
}
