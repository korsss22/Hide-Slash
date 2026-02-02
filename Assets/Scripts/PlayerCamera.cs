using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.PlayerLoop;
using Mirror.BouncyCastle.Crypto.Parameters;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.LowLevel;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCamera : NetworkBehaviour
{
    private Player player;
    private Transform virtualCam;
    private CinemachineCamera vCamera;
    private CinemachineOrbitalFollow freeLook;
    [SerializeField] private Transform camPivot;
    [Range(1f,15f)]
    [SerializeField] private float minDistance = 2f;
    [Range(1f,15f)]
    [SerializeField] private float maxDistance = 15f;
    [Range(1f,15f)]
    [SerializeField] private float defaultDistance = 5f;
    private float yaw = 0f;
    private float nowDis = 0;


    [Range(0.01f, 5f)]
    [SerializeField] float sensitivity = 1f;

    public void Init(Player player) {
        this.player = player;
        nowDis = defaultDistance;
    }

    void OnScrollWheel(InputValue value) {
        if (GameManager.Instance.IsMouseLocked) return;
        Vector2 wheel = value.Get<Vector2>();
        MoveCam(wheel);
    }

    //TODO : Fix Camera and rotate player transform
    void OnLook(InputValue value) {
        if (!isLocalPlayer) return;
        Vector2 mouse = value.Get<Vector2>();

        yaw += mouse.x * sensitivity;

        if (Input.GetKey(KeyCode.LeftAlt)) {
            player.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        } 
        
    }

    private void MoveCam(Vector2 camVec) {
        nowDis = Mathf.Clamp(
            nowDis - camVec.y,
            minDistance,
            maxDistance
        );

        float t = Mathf.InverseLerp(minDistance, maxDistance, nowDis);

        if (freeLook.OrbitStyle == CinemachineOrbitalFollow.OrbitStyles.ThreeRing)
        {
            freeLook.Orbits.Top.Radius    = Mathf.Lerp(2.5f, 6.0f, t);
            freeLook.Orbits.Top.Height    = Mathf.Lerp(3f, 4.8f, t);

            freeLook.Orbits.Center.Radius = Mathf.Lerp(3f, 9.8f, t);
            freeLook.Orbits.Center.Height = Mathf.Lerp(1.5f, 3.2f, t);

            freeLook.Orbits.Bottom.Radius = Mathf.Lerp(2f, 7.5f, t);
            freeLook.Orbits.Bottom.Height = Mathf.Lerp(0.5f, 1.6f, t);
        }
        else
        {
            freeLook.Radius = nowDis;
        }
    }

    // Invoke after Start
    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        virtualCam = GameObject.FindGameObjectWithTag("Cinemachine").transform;
        vCamera = virtualCam.gameObject.GetComponent<CinemachineCamera>();
        freeLook = virtualCam.gameObject.GetComponent<CinemachineOrbitalFollow>();
        
        if (camPivot == null) {
            vCamera.Follow = transform;
            vCamera.LookAt = transform;
        } else {
            vCamera.Follow = camPivot;
            vCamera.LookAt = camPivot;
        }
        
        PlayerInput input = player.gameObject.GetComponent<PlayerInput>();

        input.enabled = true;
        input.ActivateInput();

        GameManager.Instance.EnterLockState(LockState.LOCKED);
    }
}
