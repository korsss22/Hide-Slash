using Mirror;
using UnityEngine;

public class NetworkDebugger : NetworkBehaviour
{
    int seq = 0;

    void Log(string msg)
    {
        Debug.Log($"[{++seq}] {name}.{msg} (frame {Time.frameCount})");
    }

    void Awake() {
        DontDestroyOnLoad(gameObject);
        Log("Awake");
    }
    void Start() => Log("Start");

    public override void OnStartServer() => Log("OnStartServer");
    public override void OnStartClient() => Log("OnStartClient");
    public override void OnStartAuthority() => Log("OnStartAuthority");
    public override void OnStartLocalPlayer() => Log("OnStartLocalPlayer");
}
