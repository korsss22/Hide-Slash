using System.Collections.Generic;
using UnityEngine;

public class CursorLockManager : Singleton<CursorLockManager>
{
    private readonly Stack<LockState> lockStateStack = new();

    public LockState CurrentLockState { get; private set; } = LockState.UNLOCKED;

    public bool IsLocked => CurrentLockState == LockState.LOCKED;

    public void EnterLockState(LockState state)
    {
        lockStateStack.Push(state);
        ApplyCurrentState();
    }

    public void ExitLockState()
    {
        if (lockStateStack.Count > 0)
        {
            lockStateStack.Pop();
        }

        CurrentLockState = lockStateStack.Count > 0
            ? lockStateStack.Peek()
            : LockState.UNLOCKED;

        ApplyCurrentState();
    }

    private void ApplyCurrentState()
    {
        CurrentLockState = lockStateStack.Count > 0
            ? lockStateStack.Peek()
            : LockState.UNLOCKED;

        if (IsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}