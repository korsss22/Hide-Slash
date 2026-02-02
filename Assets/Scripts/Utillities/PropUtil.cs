using UnityEngine;

public class PropUtil : MonoBehaviour
{
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
}
