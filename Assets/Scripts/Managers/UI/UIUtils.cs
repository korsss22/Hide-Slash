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
    [SerializeField] private Image loadingBar = null;

    private Coroutine loadingBarCoroutine;

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

        if (loadingBar == null) {
            Debug.LogError("loadingBar Load failed!");
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

    public static void StartLoadingBar()
    {
        if (Instance == null || Instance.loadingBar == null) {
            Debug.LogError("UIUtil Instance or loadingBar is not initialized...");
            return;
        }

        if (Instance.loadingBarCoroutine != null) {
            Instance.StopCoroutine(Instance.loadingBarCoroutine);
        }

        if (!Instance.loadingBar.gameObject.activeSelf)
        {
            Instance.loadingBar.gameObject.SetActive(true);
        }

        Instance.loadingBarCoroutine = Instance.StartCoroutine(LoadingBarAsync());
    }

    private static void SetLoadingBarProgress(float progress) {
        if (Instance == null || Instance.loadingBar == null) {
            Debug.LogError("UIUtil Instance or loadingBar is not initialized...");
            return;
        }

        Instance.loadingBar.fillAmount = progress;
    }

    private static void SetLoadingBarActive(bool isActive) {
        if (Instance == null || Instance.loadingBar == null) {
            Debug.LogError("UIUtil Instance or loadingBar is not initialized...");
            return;
        }
        if (!isActive) Instance.loadingBar.fillAmount = 0f;
        Instance.loadingBar.gameObject.SetActive(isActive);
    }

    private static IEnumerator LoadingBarAsync()
    {
        if (NetworkController.Instance == null) {
            Debug.LogError("NetworkController Instance is not initialized...");
            yield break;
        }

        float progress = 0f;

        while (progress < 0.85f)
        {
            progress = NetworkController.Instance.GetSceneProgress();
            SetLoadingBarProgress(progress);
            yield return null;
        }

        SetLoadingBarActive(false);

        yield break;
    }
}
