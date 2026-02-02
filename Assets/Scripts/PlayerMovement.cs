using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Vector2 inputVec;
    [SerializeField] private Vector3 moveVec;
    [Range(0.01f, 5f)]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float nowSpeed = 0f;
    [Range(0.01f, 20f)]
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private LayerMask mapLayer;
    private CapsuleCollider body;
    [SerializeField] float bodyRadius = 0.3f;
    [SerializeField] float bodyHeight = 1.8f;
    [SerializeField] float skinWidth = 0.02f;
    [SerializeField] int maxSlideCount = 7;
    private Vector3 bodyCenter;
    private float jumpForce = 4f;
    private float radiusOffset = 0.2f;

    private Player player;
    private Rigidbody rb;

    public void Init(Player player) {
        this.player = player;
        nowSpeed = walkSpeed;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        body = player.collision.GetComponent<CapsuleCollider>();
        bodyRadius = body.radius;
        bodyHeight = body.height;
        bodyCenter = body.center;
    }

    private void Update()
    {
        if (player.Cam == null || !isLocalPlayer) return;
        if (!GameManager.Instance.IsMouseLocked) return;

        Vector3 moveDir =
            player.Cam.transform.right * inputVec.x +
            player.Cam.transform.forward * inputVec.y;

        moveDir.y = 0f;
        if (moveDir.sqrMagnitude < 0.0001f)
            return;

        moveDir.Normalize();

        Vector3 remainingMove = moveDir * nowSpeed * Time.deltaTime;
        Vector3 currentPos = transform.position;

        //int checkLayer = ~LayerMask.GetMask("Map"); //충돌 검사할 레이어

        // 2️⃣ Collide & Slide 루프
        for (int i = 0; i < maxSlideCount; i++)
        {
            if (remainingMove.sqrMagnitude < 0.00001f)
                break;

            GetCapsulePoints(currentPos, out Vector3 bottom, out Vector3 top);

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

        bool isTurnable = moveDir.sqrMagnitude > 0.0001f && !player.isTransformed;

        if (isTurnable)
        {   
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                8f * Time.deltaTime
            );
        }

        transform.position = currentPos;
    }

    void GetCapsulePoints(Vector3 pos, out Vector3 bottom, out Vector3 top)
    {
        Vector3 up = Vector3.up;

        bottom = pos + up * bodyRadius;
        top = pos + up * (bodyHeight - bodyRadius);
    }
    
    void OnMove(InputValue value) {
        if (!GameManager.Instance.IsMouseLocked) return;
        inputVec = value.Get<Vector2>();
    }

    void OnJump(InputValue value) {
        if (!GameManager.Instance.IsMouseLocked) return;
        bool isPress = value.isPressed;

        if (isGround() && isPress) {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnSprint(InputValue value) {
        if (!GameManager.Instance.IsMouseLocked) return;
        bool isPress = value.isPressed;
        nowSpeed = walkSpeed;
        if (isPress) {
            nowSpeed = sprintSpeed;
        } else {
            nowSpeed = walkSpeed;
        }
        
    }

    private bool isGround() {
        return Physics.CheckSphere(
            transform.position + new Vector3(0f, 0.2f, 0f), 0.4f, mapLayer
        );
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.4f);

        Vector3 nextPosition = player.collision.position + nowSpeed * moveVec * Time.fixedDeltaTime;

        DrawCapsuleGizmo(nextPosition, bodyHeight, bodyRadius, Color.green);
        
    }

    void DrawCapsuleGizmo(
    Vector3 center,
    float height,
    float radius,
    Color color
) {
    Gizmos.color = color;

    float halfHeight = height * 0.5f - radius;

    Vector3 top    = center + Vector3.up * halfHeight;
    Vector3 bottom = center - Vector3.up * halfHeight;

    // 위 / 아래 반구
    Gizmos.DrawWireSphere(top, radius);
    Gizmos.DrawWireSphere(bottom, radius);

    // 옆선 (4방향)
    Gizmos.DrawLine(top + Vector3.forward * radius, bottom + Vector3.forward * radius);
    Gizmos.DrawLine(top + Vector3.back    * radius, bottom + Vector3.back    * radius);
    Gizmos.DrawLine(top + Vector3.left    * radius, bottom + Vector3.left    * radius);
    Gizmos.DrawLine(top + Vector3.right   * radius, bottom + Vector3.right   * radius);
}
}
