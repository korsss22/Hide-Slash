using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PropDatabase))]
public class PropDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Sync Prop IDs (Database 기준)"))
        {
            SyncPropIds((PropDatabase)target);
        }
    }

    private void SyncPropIds(PropDatabase database)
    {
        if (database.propDatas == null)
        {
            Debug.LogError("propDatas is null");
            return;
        }

        Undo.RecordObject(database, "Sync Prop IDs");

        for (int i = 0; i < database.propDatas.Count; i++)
        {
            PropIdentify propData = database.propDatas[i];

            if (propData == null)
            {
                Debug.LogWarning($"[Index {i}] PropIdentify is null");
                continue;
            }

            // --- PropIdentify SO ---
            Undo.RecordObject(propData, "Sync PropIdentify ID");
            propData.propID = i;
            EditorUtility.SetDirty(propData);

            // --- Prefab / PropParent ---
            if (propData.propPrefab == null)
            {
                Debug.LogWarning($"[Index {i}] propPrefab is null");
                continue;
            }

            PropParent propParent =
                propData.propPrefab.GetComponent<PropParent>();

            if (propParent == null)
            {
                Debug.LogWarning(
                    $"[Index {i}] Prefab '{propData.propPrefab.name}' has no PropParent"
                );
                continue;
            }

            Undo.RecordObject(propParent, "Sync PropParent ID");
            propParent.propID = i;
            EditorUtility.SetDirty(propParent);
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log("✅ Prop ID Sync Complete (PropDatabase 기준)");
    }
}
