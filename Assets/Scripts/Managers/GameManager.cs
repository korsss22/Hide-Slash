using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum GameState {
    MAIN_MENU,
    PLAYING
}

public enum LockState {
    LOCKED = 0,
    UNLOCKED
}

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentGameState { get; private set; }
    public LockState CurrentLockState { get; private set; }
    public Dictionary<GameState, LockState> MouseLockWhen = new() {
        {GameState.MAIN_MENU, LockState.UNLOCKED},
        {GameState.PLAYING, LockState.LOCKED}
    };
    private Stack<LockState> mouseLockStack = new();

    public bool IsMouseLocked { 
        get {
            if (CurrentLockState == LockState.LOCKED) return true;
            else return false;
        }
    }

    private void Update() {
        if (Instance == null) {
            Debug.LogWarning("GameManager Instance is null");
        } else {
            Debug.LogWarning("GameManager Instance is not null");
        }
    }

    protected override void Awake() {
        base.Awake();
        EnterGameState(GameState.MAIN_MENU);
        EnterLockState(LockState.UNLOCKED);
    }

    public void EnterGameState(GameState nowState) {
        CurrentGameState = nowState;
    }

    public void EnterLockState(LockState nowState) {
        mouseLockStack.Push(nowState);
        CurrentLockState = nowState;
        SetMouseLockByState();
    }

    public void ExitLockState() {
    if (mouseLockStack.Count > 0)
        mouseLockStack.Pop();

        CurrentLockState = mouseLockStack.Count > 0 ? mouseLockStack.Peek() : LockState.UNLOCKED;
        SetMouseLockByState();
    }

    private void SetMouseLockByState() {
        if (IsMouseLocked) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
