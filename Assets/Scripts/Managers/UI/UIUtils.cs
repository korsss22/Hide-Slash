using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    [SerializeField] private DebugConsole debugConsole = null; 
    [SerializeField] private PausePanel pausePanel = null;   

    override protected void Awake() {
        base.Awake();

        if (debugConsole == null) {
            Debug.LogError("debugConsole Load failed!");
            return;
        }

        if (pausePanel == null) {
            Debug.LogError("pausePanel Load failed!");
            return;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (pausePanel.gameObject.activeSelf) {
                CloseUI(pausePanel.gameObject);
            } else {
                PopUI(pausePanel.gameObject, LockState.UNLOCKED);
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

        debugConsole.debugQueue.Enqueue(new DebugData{title = title, content = content});
        if (!debugConsole.isShowing) {
            debugConsole.Show();
        }
    }

    public static void PrintUI(DEBUG_TYPE type, string content) {
        if (Instance == null) {
            Debug.LogError("UIUtil Instance is not initialized...");
            return;
        }
        Instance.PrintInternal(type, content);
    }

    public static void PopUI(GameObject obj, LockState lockState) {
        GameManager.Instance.EnterLockState(lockState);
        obj.SetActive(true);
    }

    public static void CloseUI(GameObject obj) {
        GameManager.Instance.ExitLockState();
        obj.SetActive(false);
    }
}
