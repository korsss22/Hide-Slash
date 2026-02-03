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
    public bool isTransformed = false;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<PlayerCamera>();
        transformSystem = GetComponentInChildren<PropTransformSystem>();

        playerMovement.Init(this);
        playerCamera.Init(this);
    }
}