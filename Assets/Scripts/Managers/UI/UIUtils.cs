using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Pkix;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum DEBUG_TYPE {
    ALERT = 0,
    WARNING,
    ERROR
}

public struct DebugData {
    public string title;
    public string content;
}

public class UIUtils : Singleton<UIUtils>
{
    [SerializeField] private DebugConsole DebugConsole = null; 
    [SerializeField] private PausePanel PausePanel = null;   
    [SerializeField] private HoldInteractionUI HoldInteractionUI = null;

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
    }

    void Start()
    {
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

    public static void PopUIWithUnlock(GameObject obj, LockState lockState) {
        GameManager.Instance.EnterLockState(lockState);
        obj.SetActive(true);
    }

    public static void CloseUIWithLock(GameObject obj) {
        GameManager.Instance.ExitLockState();
        obj.SetActive(false);
    }

    public static HoldInteractionUI GetHoldUI(Action onLoaded = null) {
        HoldInteractionUI holdUI = Instance.HoldInteractionUI;
        holdUI.onHoldComplete = onLoaded;
        return holdUI;
    }
}
