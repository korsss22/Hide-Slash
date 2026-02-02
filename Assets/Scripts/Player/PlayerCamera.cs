using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Unity.Cinemachine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCamera : NetworkBehaviour
{
    private Player player;

    private Transform cineMachine;
    private CinemachineCamera virtualCamera;
    private CinemachineOrbitalFollow orbitCam;
    [SerializeField] private Transform camPivot;

    [Range(1f,15f)][SerializeField] private float minDistance = 2f;
    [Range(1f,15f)][SerializeField] private float maxDistance = 15f;
    [Range(1f,15f)][SerializeField] private float defaultDistance = 5f;
    [Range(0.01f, 5f)][SerializeField] float sensitivity = 1f;

    private float yaw = 0f;
    private float nowDis = 0;

    public void Init(Player player) {
        this.player = player;
        nowDis = defaultDistance;
    }

    // Invoke after Start
    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        
        InitializeCamera();
        
        PlayerInput input = player.gameObject.GetComponent<PlayerInput>();
        input.enabled = true;
        input.ActivateInput();
    }

    private void InitializeCamera() {
        player.Cam = Camera.main;
        cineMachine = GameObject.FindGameObjectWithTag("Cinemachine").transform;
        virtualCamera = cineMachine.gameObject.GetComponent<CinemachineCamera>();
        orbitCam = cineMachine.gameObject.GetComponent<CinemachineOrbitalFollow>();
        
        if (camPivot == null) {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        } else {
            virtualCamera.Follow = camPivot;
            virtualCamera.LookAt = camPivot;
        }
    }

/*----------------Camera Callback----------------*/

    void OnScrollWheel(InputValue value) {
        if (!isLocalPlayer) return;

        Vector2 wheel = value.Get<Vector2>();
        MoveCam(wheel);
    }

    private void MoveCam(Vector2 camVec) {
        nowDis = Mathf.Clamp(
            nowDis - camVec.y,
            minDistance,
            maxDistance
        );

        float t = Mathf.InverseLerp(minDistance, maxDistance, nowDis);

        if (orbitCam.OrbitStyle == CinemachineOrbitalFollow.OrbitStyles.ThreeRing)
        {
            orbitCam.Orbits.Top.Radius    = Mathf.Lerp(2.5f, 6.0f, t);
            orbitCam.Orbits.Top.Height    = Mathf.Lerp(3f, 4.8f, t);

            orbitCam.Orbits.Center.Radius = Mathf.Lerp(3f, 9.8f, t);
            orbitCam.Orbits.Center.Height = Mathf.Lerp(1.5f, 3.2f, t);

            orbitCam.Orbits.Bottom.Radius = Mathf.Lerp(2f, 7.5f, t);
            orbitCam.Orbits.Bottom.Height = Mathf.Lerp(0.5f, 1.6f, t);
        }
        else
        {
            orbitCam.Radius = nowDis;
        }
    }

    void OnLook(InputValue value) {
        if (!isLocalPlayer) return;

        Vector2 mouse = value.Get<Vector2>();
        yaw += mouse.x * sensitivity;
        if (Input.GetKey(KeyCode.LeftAlt)) {
            player.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        } 
        
    }
}
