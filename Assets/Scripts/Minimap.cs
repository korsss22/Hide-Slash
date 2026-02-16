using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Camera RenderCamera;
    [SerializeField] private Transform TargetObj;
    [SerializeField] private Vector3 offset = new(0f, 70f, 0f);
    [SerializeField] private LayerMask HiderMask;
    [SerializeField] private LayerMask SeekerMask;
    [SerializeField] private bool isRotate = false;

    private void Awake() {
        if (TargetObj == null) return;
    }

    private void LateUpdate() {
        if (!TargetObj) return;
        if (isRotate)
        {
            Vector3 euler = Camera.main.transform.rotation.eulerAngles;
            euler.x = 90f;
            RenderCamera.transform.rotation = Quaternion.Euler(euler);
        }
        RenderCamera.transform.position = TargetObj.position + offset;
    }

    public void SetTarget(bool isHider, Transform target) {
        TargetObj = target;
        RenderCamera.cullingMask = isHider ? HiderMask : SeekerMask;
    }
}
