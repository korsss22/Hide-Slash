using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCamera))]
[RequireComponent(typeof(PlayerCombat))]
public class Player : NetworkBehaviour
{   
    [Header("Fields")]
    private PlayerMovement playerMovement;
    private PlayerCamera playerCamera;
    private PropTransformSystem transformSystem;
    private PlayerCombat playerCombat;
    
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
    public Action OnPushing;
    
    
    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<PlayerCamera>();
        transformSystem = GetComponentInChildren<PropTransformSystem>();
        playerCombat = GetComponent<PlayerCombat>();

        playerCamera.RegisterEvent(this);
        playerMovement.RegisterEvent(this);
        transformSystem.RegisterEvent(this);
        playerCombat.RegisterEvent(this);

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

    [Command]
    public void CmdPush(List<NetworkIdentity> identities, float pushPower) {
        if (identities == null) return;

        foreach (NetworkIdentity identity in identities)
        {
            if (!identity.TryGetComponent(out Rigidbody rb)) return;

            Vector3 opposition = (rb.position - transform.position).normalized;
            Vector3 pushVec = opposition * pushPower;

            rb.AddForce(pushVec, ForceMode.Impulse);
            Debug.Log("Pushed Object");
        }
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

    public void OnPushingObject() {
        OnPushing?.Invoke();
    }

// mirror callbacks
    public override void OnStartLocalPlayer()
    {
        this.enabled = true;

        GameManager.Instance.OnLocalPlayerSpawned?.Invoke(this);
    }

    private void OnDestroy() {
        playerCamera.UnRegisterEvent();
        playerMovement.UnRegisterEvent();
        transformSystem.UnRegisterEvent();
        playerCombat.UnRegisterEvent();
    }
}