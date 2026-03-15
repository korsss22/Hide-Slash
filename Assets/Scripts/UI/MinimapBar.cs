using UnityEngine;

public class MinimapBar : MonoBehaviour
{
    private Minimap minimap;

    private void Awake() {
        minimap = GetComponentInParent<Minimap>();
        if (!minimap) Debug.LogError("unable use minimap bar..."); return;
    }

    public void ToggleRotate() {
        minimap.isRotate =  !minimap.isRotate;
    }
}
