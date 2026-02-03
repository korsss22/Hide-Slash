using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]private Vector2 inputVec;
    [SerializeField]private Vector3 moveVec;

    [Range(0.01f, 5f)][SerializeField] private float walkSpeed = 2f;
    [Range(0.01f, 20f)][SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float nowSpeed = 0f;

    [Range(1f, 10f)][SerializeField] private float jumpForce = 4f;
    
    [SerializeField] private LayerMask mapLayer;
    
    private Player player;
    private Rigidbody rb;
    private CapsuleCollider body;
    private float bodyRadius = 0.3f;
    private float bodyHeight = 1.8f;
    [SerializeField]private float skinWidth = 0.02f;

    [SerializeField] int maxSlideCount = 7;

    public void Init(Player player) {
        this.player = player;
        nowSpeed = walkSpeed;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        body = player.collision.GetComponent<CapsuleCollider>();
        bodyRadius = body.radius;
        bodyHeight = body.height;
    }

    private void FixedUpdate()
    {
        if (player.Cam == null || !isLocalPlayer) return;

        Vector3 moveDir = GetMoveDir();

        Vector3 currentPos = CollideAndSlideMove(moveDir);

        bool isTurnable = moveDir.sqrMagnitude > 0.0001f && !player.isTransformed;

        if (isTurnable)
        {   
            TurnLookWay(moveDir, 8f);
        }

        rb.MovePosition(currentPos);
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
            player.Cam.transform.right * inputVec.x +
            player.Cam.transform.forward * inputVec.y;

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

/*-------------------PlayerInput Callbacks---------------------*/
    
    public void OnMove(InputAction.CallbackContext context) {
        Debug.Log(isLocalPlayer);
        if (!isLocalPlayer) return;
        inputVec = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context) {
        if (!isLocalPlayer) return;

        bool isPress = context.ReadValue<float>() > 0.5f;        
        nowSpeed = isPress ? sprintSpeed : walkSpeed;
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (!isLocalPlayer) return;
        
        bool isPress = context.ReadValue<float>() > 0.5f;

        if (Utils.IsGround(transform.position, mapLayer) && isPress) {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
