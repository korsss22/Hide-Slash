using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCamera))]
public class Player : NetworkBehaviour
{
    public Camera Cam { get; private set; }
    private PlayerMovement playerMove;
    private PlayerCamera playerCamera;
    private PropTransformSystem transformSystem;
    public Transform collision;
    public Transform Mesh;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    private Mesh originalMesh;
    private Material originalMaterial;
    public bool isCollision = false;
    public bool isTransformed = false;

    private void Awake() {
        meshFilter = GetComponentInChildren<MeshFilter>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (Mesh == null) Mesh = meshFilter.transform;
        originalMesh = meshFilter.mesh;
        originalMaterial = meshRenderer.material;

        Cam = Camera.main;
        playerMove = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<PlayerCamera>();
        transformSystem = GetComponentInChildren<PropTransformSystem>();

        playerMove.Init(this);
        playerCamera.Init(this);
        transformSystem.Init(this);
    }

    public void OnInteract(InputValue value) {
        if (isTransformed) {
            transformSystem.RevertTransform(value);
        } else {
            transformSystem.Transform(value);
        }
    }
}