using System;
using Mirror;
using UnityEngine;

public enum DEBUG_TYPE {
    ALERT = 0,
    WARNING,
    ERROR
}

public struct DebugData {
    public string title;
    public string content;
}

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private DebugConsole DebugConsole = null; 
    [SerializeField] private PausePanel PausePanel = null;   
    [SerializeField] private HoldInteractionUI HoldInteractionUI = null;
    [SerializeField] private LoadingUI LoadingUI = null;
    private Transform canvasTransform;

    override protected void Awake() {
        base.Awake();

        if (DebugConsole == null) {
            Debug.LogError("debugConsole Load failed!");
            return;
        }

        if (PausePanel == null) {
            Debug.LogError("pausePanel Load failed!");
            return;
        }

        if (HoldInteractionUI == null) {
            Debug.LogError("holdInteractionUI Load failed!");
            return;
        }

        if (LoadingUI == null) {
            Debug.LogError("loadingUI Load failed!");
            return;
        }

        canvasTransform = transform.GetComponentInChildren<Canvas>(true).transform;
    }

    void Start()
    {
        StartCoroutine(LoadingUI.ShowLogo());
    }

    private void OnEnable() {
        GameManager.Instance.OnLocalPlayerSpawned += OnLocalPlayerSpawned;
    }

    private void OnDisable() {
        if (GameManager.Instance == null) {
            GameManager.Instance.OnLocalPlayerSpawned -= OnLocalPlayerSpawned;
        }
    }

    private void OnLocalPlayerSpawned(Player player) {
        TurnGameUIOn();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (PausePanel.gameObject.activeSelf) {
                CloseUIWithLock(PausePanel.gameObject);
            } else {
                PopUIWithUnlock(PausePanel.gameObject, LockState.UNLOCKED);
            }
        }
    }

    private void ToggleInternal(GameObject obj) {
        obj.SetActive(!obj.activeSelf);
    }

    //TODO: Add Cursor state in GameManager or such like that
    private void PrintInternal(DEBUG_TYPE type, string content) {
        string title = type == DEBUG_TYPE.ALERT ?
        "Alert" : type == DEBUG_TYPE.WARNING
        ? "Warning" : "Error";

        DebugConsole.debugQueue.Enqueue(new DebugData{title = title, content = content});
        if (!DebugConsole.isShowing) {
            DebugConsole.Show();
        }
    }

    public static void PrintUI(DEBUG_TYPE type, string content) {
        if (Instance == null) {
            Debug.LogError("UIUtil Instance is not initialized...");
            return;
        }
        Instance.PrintInternal(type, content);
    }

    public static void PopUIWithUnlock(GameObject obj, LockState lockState)
    {
        CursorLockManager.Instance.EnterLockState(lockState);
        obj.SetActive(true);
    }

    public static void CloseUIWithLock(GameObject obj)
    {
        CursorLockManager.Instance.ExitLockState();
        obj.SetActive(false);
    }

    public static HoldInteractionUI GetHoldUI(Action onLoaded = null) {
        HoldInteractionUI holdUI = Instance.HoldInteractionUI;
        holdUI.onHoldComplete = onLoaded;
        return holdUI;
    }

    public void TurnGameUIOn() {
        Transform GameUI = canvasTransform.Find("GameUI");
        if (GameUI == null) {
            Debug.LogError("failed to find GameUI");
            return;
        }
        GameUI.gameObject.SetActive(true);
        Transform local = NetworkClient.localPlayer.transform;
        if (GameObject.FindGameObjectWithTag("Minimap").TryGetComponent(out Minimap minimap)) minimap.SetTarget(true, local);
    }
}
