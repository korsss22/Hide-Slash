using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MAIN_MENU,
    PLAYING
}

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentGameState { get; private set; }

    [SerializeField] private PropDatabase database;

    public event Action OnGameStart;
    public event Action OnGamePlaying;
    public event Action OnGameEnd;
    public event Action<Player> OnLocalPlayerSpawned;

    protected override void Awake()
    {
        base.Awake();

        PropUtil.Init(database);

        EnterGameState(GameState.MAIN_MENU);

        OnGameStart += EnterPlayMode;
    }

    public void GameStart()
    {
        OnGameStart?.Invoke();
    }

    private void EnterPlayMode()
    {
        EnterGameState(GameState.PLAYING);

        CursorLockManager.Instance.EnterLockState(
            LockState.LOCKED
        );
    }

    public void EnterGameState(GameState nowState)
    {
        CurrentGameState = nowState;
    }

    public void NotifyLocalPlayerSpawned(Player player)
    {
        OnLocalPlayerSpawned?.Invoke(player);
    }

    private void OnDestroy()
    {
        OnGameStart -= EnterPlayMode;
    }
}