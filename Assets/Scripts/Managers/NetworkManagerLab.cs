using UnityEngine;
using Mirror;
using System.Collections;
using System;
using UnityEngine.SceneManagement;


public class NetworkManagerLab : NetworkManager
{
    public override void OnStartClient() {
        base.OnStartClient();

        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "Map":
                GameManager.Instance.GameStart();
                break;
            case "MainScene":
                Debug.Log("leaved game...");
                break;
            default:
                break;
        }
    }
}
