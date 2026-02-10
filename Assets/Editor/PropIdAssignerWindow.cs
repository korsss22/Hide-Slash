using UnityEditor;
using UnityEngine;

public class PropIdAssignerWindow : EditorWindow
{
    private const string LAST_ID_KEY = "PropIdAssigner_LastId";
    private int nextId;

    [MenuItem("Tools/Prop ID Assigner")]
    public static void Open()
    {
        GetWindow<PropIdAssignerWindow>("Prop ID Assigner");
    }

    private void OnEnable()
    {
        nextId = EditorPrefs.GetInt(LAST_ID_KEY, 0);
    }

    private void OnGUI()
    {
        GUILayout.Label("Selected Prefabs → PropIdentify 자동 생성", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        nextId = EditorGUILayout.IntField("Next Prop ID", nextId);

        EditorGUILayout.Space();

        if (GUILayout.Button("Assign IDs to Selected Prefabs"))
        {
            Assign();
        }
    }

    private void Assign()
    {
        var selectedObjects = Selection.objects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("선택된 오브젝트가 없습니다.");
            return;
        }

        foreach (var obj in selectedObjects)    
        {
            if (!(obj is GameObject prefab))
                continue;

            // PropIdentify SO 생성
            var propData = ScriptableObject.CreateInstance<PropIdentify>();
            propData.propID = nextId++;
            propData.propPrefab = prefab;

            string path = $"Assets/PropData/Prop_{propData.propID}.asset";

            AssetDatabase.CreateAsset(propData, path);
            EditorUtility.SetDirty(propData);

            Debug.Log($"Created PropIdentify: ID={propData.propID}, Prefab={prefab.name}");
        }

        EditorPrefs.SetInt(LAST_ID_KEY, nextId);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
