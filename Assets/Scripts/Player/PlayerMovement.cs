using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
// fields
    private Player Player;
    private Camera playerCam;
    private Vector2 inputVec;
    private Vector3 moveVec;
    private float nowSpeed = 0f;
    private Rigidbody rb;
    private CapsuleCollider body;
    private float bodyRadius = 0.3f;
    private float bodyHeight = 1.8f;

// adjustable variables
    [Header("adjustable variables")]
    [Range(0.01f, 5f)][SerializeField] private float walkSpeed = 2f;
    [Range(0.01f, 20f)][SerializeField] private float sprintSpeed = 4f;
    [Range(1f, 10f)][SerializeField] private float jumpForce = 4f;
    [SerializeField] private LayerMask mapLayer;
    [SerializeField]private float skinWidth = 0.02f;
    [SerializeField] int maxSlideCount = 7;

// assignable variables
    [SerializeField] private Collider colliderBody;

    void Awake() {
        nowSpeed = walkSpeed;
        playerCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        body = colliderBody.GetComponent<CapsuleCollider>();
        bodyRadius = body.radius;
        bodyHeight = body.height;
    }

    [Client]
    private void FixedUpdate()
    {
        if (Player == null) {
            Debug.LogError("player is not registered yet");
            return;
        }
        Move();
    }
    
// Events
    public void RegisterEvent(Player player) {
        this.Player = player;
        
        Player.OnFixChanged += OnFixed;
        Player.OnRunningChanged += OnRunning;
    }

    public void UnRegisterEvent() {
        Player.OnFixChanged -= OnFixed;
        Player.OnRunningChanged -= OnRunning;
    }

    private void OnFixed(bool oldValue, bool newValue) {
        rb.isKinematic = newValue;
    }

    private void OnRunning(bool oldValue, bool newValue) {
        nowSpeed = newValue ? sprintSpeed : walkSpeed;
    }

// Movement implements
    private void Move() {
        if (!isLocalPlayer) {
            Debug.Log($"{gameObject.name} : not Local Player"); 
            return;
        }
        
        if (Player.IsFixed) {
            return;
        }

        if (playerCam == null) {
            Debug.Log($"{gameObject.name} : Cam is Null");
            return;
        }

        Vector3 moveDir = GetMoveDir();

        Vector3 currentPos = CollideAndSlideMove(moveDir);

        bool isTurnable = moveDir.sqrMagnitude > 0.0001f && !Player.IsTransformed;

        if (isTurnable)
        {   
            TurnLookWay(moveDir, 8f);
        }

        rb.MovePosition(currentPos);

        // if (Vector3.Distance(currentPos, rb.position) > 0.1f) {
        //     Debug.LogWarning("Position Mismatch! Something is snapping the player back.");
        // }
    }

    public void TurnLookWay(Vector3 moveDir, float turnSpeed) {
        Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                turnSpeed * Time.fixedDeltaTime
            );
    }

    private Vector3 GetMoveDir() {
        Vector3 moveDir =
            playerCam.transform.right * inputVec.x +
            playerCam.transform.forward * inputVec.y;

        moveDir.y = 0f;
        if (moveDir.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        moveDir.Normalize();
        return moveDir;
    }

    private Vector3 CollideAndSlideMove(Vector3 moveDir) {
        Vector3 remainingMove = Time.fixedDeltaTime * nowSpeed * moveDir;
        Vector3 currentPos = rb.position;
        
        for (int i = 0; i < maxSlideCount; i++)
        {
            if (remainingMove.sqrMagnitude < 0.00001f)
                break;

            Utils.GetCapsulePoints(currentPos, bodyRadius, bodyHeight, out Vector3 bottom, out Vector3 top);

            Vector3 dir = remainingMove.normalized;
            float dist = remainingMove.magnitude;

            bool hit = Physics.CapsuleCast(
                bottom,
                top,
                bodyRadius - skinWidth,
                dir,
                out RaycastHit hitInfo,
                dist,
                mapLayer,
                QueryTriggerInteraction.Ignore
            );

            if (!hit)
            {
                currentPos += remainingMove;
                break;
            }

            float moveDist = Mathf.Max(hitInfo.distance - skinWidth, 0f);
            currentPos += dir * moveDist;

            remainingMove -= dir * moveDist;

            remainingMove = Vector3.ProjectOnPlane(remainingMove, hitInfo.normal);
        }

        return currentPos;
    }


// mirror callbacks
    public override void OnStartLocalPlayer()
    {
        name = "Player"+Time.time;
        PlayerInput input = GetComponent<PlayerInput>();

        input.enabled = true;

        input.ActivateInput();

        this.enabled = true;
    }

/*-------------------PlayerInput Callbacks---------------------*/
    
    public void OnMove(InputAction.CallbackContext context) {
        if (!isLocalPlayer) return;
        inputVec = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context) {
        if (!isLocalPlayer) return;
        bool isRunning = context.ReadValue<float>() > 0.5f;        
        Player.CmdSetRunning(isRunning);
    }

    

    public void OnJump(InputAction.CallbackContext context) {
        if (!isLocalPlayer) return;
        bool isPress = context.ReadValue<float>() > 0.5f;

        if (Utils.IsGround(transform.position, mapLayer) && isPress) {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void OnFix(InputAction.CallbackContext context) {
        if (!isLocalPlayer) return;

        if (!Player.IsTransformed) return; 
        if (context.started) {
            Player.CmdRequestFix(!Player.IsFixed);
        }
    }
}