using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using System;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCamera))]
public class Player : NetworkBehaviour
{
    public Camera Cam;
    private PlayerMovement playerMovement;
    private PlayerCamera playerCamera;
    private PropTransformSystem transformSystem;
    private Rigidbody rb;
    public Transform collision;
    public Transform Mesh;
    public bool isOnCollision = false;
    [SyncVar(hook = nameof(OnTransformChanged))] public int transformObjId = -1;
    [SyncVar(hook = nameof(OnFixed))] public bool isFixed = false;
    [SyncVar(hook = nameof(OnPlayerRunningChanged))] public bool IsRunning = false;

    public Action<bool> OnRunningChanged;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<PlayerCamera>();
        transformSystem = GetComponentInChildren<PropTransformSystem>();
    }

    public bool IsTransformed() {
        return transformObjId != -1;
    }

    [Command]
    public void CmdRequestTransform(int newValue) {
        transformObjId = newValue;
    }

    [Command]
    public void CmdRequestFix(bool newValue) {
        isFixed = newValue;
    }

    private void OnTransformChanged(int _, int newValue) {
        if (newValue != -1) {
            transformSystem.Apply(newValue);
        } else {
            transformSystem.Revert();
        }
    }

    private void OnFixed(bool _, bool newValue) {
        Debug.Log("isFixed is changed. now : "+newValue);
        
        if (newValue) {
            rb.isKinematic = true;
        } else {
            rb.isKinematic = false;
        }
    }

    private void OnPlayerRunningChanged(bool oldValue, bool newValue) {
        OnRunningChanged?.Invoke(newValue);
    }

    public override void OnStartLocalPlayer()
    {
        this.enabled = true;
        playerCamera.RegisterEvent(this);
        GameManager.Instance.OnLocalPlayerSpawned?.Invoke(this);
    }
}