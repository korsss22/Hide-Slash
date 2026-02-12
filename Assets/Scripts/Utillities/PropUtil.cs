using UnityEngine;

public static class PropUtil
{
    private static PropDatabase database;

    public static void Init(PropDatabase data) {
        database = data;
    }

    public static PropIdentify GetPropById(int propId) {
        if (database == null) return null;

        PropIdentify propIdentify = database.propDatas[propId];
        
        if (propIdentify.propID != propId) {
            Debug.LogError("PropId does not match.");
            return null;
        }

        return propIdentify;
    }

    public static GameObject GetPropParent(GameObject obj) {
        PropParent parent = obj.GetComponentInParent<PropParent>();
        return parent ? parent.gameObject : null;
    }

    public static void SetPropLayerInChildren(GameObject parent, LayerMask layer) {
        parent.layer = layer;
        
        foreach (Transform child in parent.transform)
        {
            SetPropLayerInChildren(child.gameObject, layer);
        }
    }

    public static void SetAllChildColliders(GameObject root, bool enabled)
    {
        if (root == null) return;

        Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
        foreach (var col in colliders)
        {
            if (col != null)
                col.enabled = enabled;
        }
    }
}
