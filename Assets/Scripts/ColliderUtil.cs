using UnityEngine;

public static class ColliderUtil
{
    /// <summary>
    /// source 오브젝트의 Collider를 target 오브젝트에 동일하게 생성 후 반환
    /// </summary>
    public static Collider CopyCollider(Collider source, GameObject target)
    {
        if (source == null || target == null) return null;

        Collider newCol = null;

        if (source is BoxCollider box)
        {
            BoxCollider col = target.AddComponent<BoxCollider>();
            col.center = box.center;
            col.size = box.size;
            col.isTrigger = box.isTrigger;
            newCol = col;
        }
        else if (source is SphereCollider sphere)
        {
            SphereCollider col = target.AddComponent<SphereCollider>();
            col.center = sphere.center;
            col.radius = sphere.radius;
            col.isTrigger = sphere.isTrigger;
            newCol = col;
        }
        else if (source is CapsuleCollider capsule)
        {
            CapsuleCollider col = target.AddComponent<CapsuleCollider>();
            col.center = capsule.center;
            col.radius = capsule.radius;
            col.height = capsule.height;
            col.direction = capsule.direction;
            col.isTrigger = capsule.isTrigger;
            newCol = col;
        }
        else if (source is MeshCollider mesh)
        {
            MeshCollider col = target.AddComponent<MeshCollider>();
            col.sharedMesh = mesh.sharedMesh;
            //col.convex = mesh.convex;
            col.convex = true;
            col.isTrigger = mesh.isTrigger;
            newCol = col;
        }
        else
        {
            Debug.LogWarning("Unsupported collider type: " + source.GetType());
        }

        // 복사 후 위치/회전/스케일은 부모 Transform에 따라 자동 적용
        return newCol;
    }
}