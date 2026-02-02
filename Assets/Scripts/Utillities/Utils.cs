using UnityEngine;

public class Utils : MonoBehaviour
{
    public static void DrawCapsuleGizmo(
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

    public static void GetCapsulePoints(Vector3 pos, float bodyRadius, float bodyHeight, out Vector3 bottom, out Vector3 top)
    {
        Vector3 up = Vector3.up;

        bottom = pos + up * bodyRadius;
        top = pos + up * (bodyHeight - bodyRadius);
    }

    public static bool IsGround(Vector3 playerBottom, LayerMask layer) {
        return Physics.CheckSphere(
            playerBottom + new Vector3(0f, 0.2f, 0f), 0.4f, layer
        );
    }
}
