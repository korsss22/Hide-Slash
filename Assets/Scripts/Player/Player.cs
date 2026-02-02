using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

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
    [SyncVar(hook = nameof(OnTransformChanged))] public bool isTransformed = false;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<PlayerCamera>();
        transformSystem = GetComponentInChildren<PropTransformSystem>();

        playerMovement.Init(this);
        playerCamera.Init(this);
    }

    public void OnInteract(InputValue value) {
        transformSystem.CmdTransform();
    }

    private void OnTransformChanged(bool oldValue, bool newValue) {
        transformSystem.Transform();
    }
}