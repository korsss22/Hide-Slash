using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCamera))]
public class Player : NetworkBehaviour
{   
    [Header("Fields")]
    private PlayerMovement playerMovement;
    private PlayerCamera playerCamera;
    private PropTransformSystem transformSystem;
    
    [Header("State getter")]
    public bool IsTransformed => transformObjId != -1;
    public bool IsFixed => isFixed;
    public bool IsRunning => isRunning;

    [Header("States")]
    [SyncVar(hook = nameof(OnPlayerTransformChanged))] [SerializeField] private int transformObjId = -1;
    [SyncVar(hook = nameof(OnPlayerFixed))] [SerializeField] private bool isFixed = false;
    [SyncVar(hook = nameof(OnPlayerRunningChanged))] [SerializeField] private bool isRunning = false;

    [Header("Events")]
    public Action<int, int> OnTransformChanged;    
    public Action<bool, bool> OnFixChanged;
    public Action<bool, bool> OnRunningChanged;
    
    
    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<PlayerCamera>();
        transformSystem = GetComponentInChildren<PropTransformSystem>();
    }

// command
    [Command]
    public void CmdRequestTransform(int newValue) {
        transformObjId = newValue;
    }

    [Command]
    public void CmdRequestFix(bool newValue) {
        isFixed = newValue;
    }

    [Command]
    public void CmdSetRunning(bool newValue) {
        isRunning = newValue;
    }
    
// hook callbacks
    private void OnPlayerTransformChanged(int oldValue, int newValue) {
        OnTransformChanged?.Invoke(oldValue, newValue);
    }

    private void OnPlayerFixed(bool oldValue, bool newValue) {
        OnFixChanged?.Invoke(oldValue, newValue);
    }

    private void OnPlayerRunningChanged(bool oldValue, bool newValue) {
        OnRunningChanged?.Invoke(oldValue, newValue);
    }

// mirror callbacks
    public override void OnStartLocalPlayer()
    {
        this.enabled = true;
        playerCamera.RegisterEvent(this);
        playerMovement.RegisterEvent(this);
        transformSystem.RegisterEvent(this);

        GameManager.Instance.OnLocalPlayerSpawned?.Invoke(this);
    }
}