using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SelectedColliderConverter
{
    [MenuItem("Tools/Convert To BoxCollider")]
    static void ConvertSelected()
    {
        HashSet<GameObject> targets = new HashSet<GameObject>();

        // 1️⃣ Selection 수집
        foreach (Object obj in Selection.objects)
        {
            if (obj is GameObject go)
            {
                // Hierarchy에서 선택
                targets.Add(go);
            }
            else
            {
                // Project에서 Prefab 선택
                string path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null)
                        targets.Add(prefab);
                }
            }
        }

        // 2️⃣ 변환
        foreach (var root in targets)
        {
            bool isPrefabAsset = PrefabUtility.IsPartOfPrefabAsset(root);

            foreach (var mc in root.GetComponentsInChildren<MeshCollider>(true))
            {
                if (mc.convex) continue;
                if (mc.GetComponent<BoxCollider>() != null) continue;

                Bounds b = mc.sharedMesh.bounds;

                var bc = mc.gameObject.AddComponent<BoxCollider>();
                bc.center = b.center;
                bc.size   = b.size;

                Object.DestroyImmediate(mc, true);
            }

            // 3️⃣ 저장 처리
            if (isPrefabAsset)
            {
                EditorUtility.SetDirty(root);
                PrefabUtility.SavePrefabAsset(root);
            }
            else
            {
                EditorUtility.SetDirty(root);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("선택한 오브젝트 MeshCollider → BoxCollider 변환 완료");
    }

    [MenuItem("Tools/Set Convex")]
    static void SetConvex()
    {
        if (!EditorUtility.DisplayDialog(
            "Set MeshCollider Convex",
            "선택한 오브젝트(씬/프리팹)에 포함된 모든 MeshCollider의\nConvex 옵션을 TRUE로 설정합니다.\n되돌릴 수 없습니다.",
            "Apply",
            "Cancel"))
        {
            return;
        }

        HashSet<GameObject> targets = new HashSet<GameObject>();

        // 1️⃣ Selection 수집 (Hierarchy + Project)
        foreach (Object obj in Selection.objects)
        {
            if (obj is GameObject go)
            {
                targets.Add(go);
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null)
                        targets.Add(prefab);
                }
            }
        }

        int count = 0;

        // 2️⃣ Convex 적용
        foreach (var root in targets)
        {
            bool isPrefabAsset = PrefabUtility.IsPartOfPrefabAsset(root);

            foreach (var mc in root.GetComponentsInChildren<MeshCollider>(true))
            {
                if (mc.convex) continue;

                mc.convex = true;
                EditorUtility.SetDirty(mc);
                count++;
            }

            // 3️⃣ 저장 처리
            if (isPrefabAsset)
            {
                EditorUtility.SetDirty(root);
                PrefabUtility.SavePrefabAsset(root);
            }
            else
            {
                EditorUtility.SetDirty(root);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"MeshCollider Convex 설정 완료 : {count}개");
    }

    [MenuItem("Tools/Undo Convex")]
    static void UndoConvex()
    {
        if (!EditorUtility.DisplayDialog(
            "Set MeshCollider Convex",
            "선택한 오브젝트(씬/프리팹)에 포함된 모든 MeshCollider의\nConvex 옵션을 FALSE로 설정합니다.\n되돌릴 수 없습니다.",
            "Apply",
            "Cancel"))
        {
            return;
        }

        HashSet<GameObject> targets = new HashSet<GameObject>();

        // 1️⃣ Selection 수집 (Hierarchy + Project)
        foreach (Object obj in Selection.objects)
        {
            if (obj is GameObject go)
            {
                targets.Add(go);
            }
            else
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null)
                        targets.Add(prefab);
                }
            }
        }

        int count = 0;

        // 2️⃣ Convex 적용
        foreach (var root in targets)
        {
            bool isPrefabAsset = PrefabUtility.IsPartOfPrefabAsset(root);

            foreach (var mc in root.GetComponentsInChildren<MeshCollider>(true))
            {
                if (!mc.convex) continue;

                mc.convex = false;
                EditorUtility.SetDirty(mc);
                count++;
            }

            // 3️⃣ 저장 처리
            if (isPrefabAsset)
            {
                EditorUtility.SetDirty(root);
                PrefabUtility.SavePrefabAsset(root);
            }
            else
            {
                EditorUtility.SetDirty(root);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"MeshCollider Convex 설정 완료 : {count}개");
    }
}