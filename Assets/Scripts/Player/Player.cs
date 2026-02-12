using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCamera))]
public class Player : NetworkBehaviour
{
    public Camera Cam { get;  set; }
    private PlayerMovement playerMovement;
    private PlayerCamera playerCamera;
    private PropTransformSystem transformSystem;
    public Transform collision;
    public Transform Mesh;
    public bool isOnCollision = false;
    [SyncVar(hook = nameof(OnTransformChanged))] public int transformObjId = -1;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<PlayerCamera>();
        transformSystem = GetComponentInChildren<PropTransformSystem>();

        playerMovement.Init(this);
        playerCamera.Init(this);
    }

    public bool IsTransformed() {
        return transformObjId != -1;
    }

    [Command]
    public void CmdRequestTransform(int newValue) {
        transformObjId = newValue;
    }

    private void OnTransformChanged(int _, int newValue) {
        if (newValue != -1) {
            transformSystem.Apply(newValue);
        } else {
            transformSystem.Revert();
        }
    }
}