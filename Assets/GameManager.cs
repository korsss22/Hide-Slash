using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum GameState {

}

public enum LockState {
    LOCKED = 0,
    UNLOCKED
}

public class GameManager : Singleton<GameManager>
{
    public LockState CurrentState { get; private set; }
    public bool IsMouseLocked { 
        get {
            if (CurrentState == LockState.LOCKED) return true;
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

    private Stack<LockState> mouseLockStack = new();
        
    protected override void Awake() {
        base.Awake();
        EnterLockState(LockState.UNLOCKED);
    }

    public void EnterLockState(LockState nowState) {
        mouseLockStack.Push(nowState);
        CurrentState = nowState;
        SetMouseLockByState();
    }

    public void ExitLockState() {
    if (mouseLockStack.Count > 0)
        mouseLockStack.Pop();

        CurrentState = mouseLockStack.Count > 0 ? mouseLockStack.Peek() : LockState.UNLOCKED;
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
