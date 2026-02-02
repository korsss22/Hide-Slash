using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Mirror;
using Unity.VisualScripting;

[RequireComponent(typeof(PropTransformSystem))]
public class PropOutliner : NetworkBehaviour
{
    private PropTransformSystem transformSystem;
    [SerializeField] private List<GameObject> hitObjs = new();
    private GameObject oldNearestObject = null;
    private void Awake() {
        transformSystem = GetComponent<PropTransformSystem>();
        transformSystem.OnNearestObjectChanged += OutlineOneObj;
    }

    private void Start() {
        StartCoroutine(UpdateOutline());
    }

    private IEnumerator UpdateOutline() {
        if (!isLocalPlayer) yield break;

        WaitForSeconds interval = new WaitForSeconds(0.1f);
        while (true) {
            transformSystem.UpdateNearestObj(hitObjs);
            yield return interval;
        }
    }

    private void OutlineOneObj(GameObject nearestObj) {
        if (oldNearestObject != null)
        {
            PropUtil.SetPropLayerInChildren(oldNearestObject, LayerMask.NameToLayer("Prop"));
        }

        // 2. 새 nearestObj Outline 켜기
        if (nearestObj != null)
        {
            PropUtil.SetPropLayerInChildren(nearestObj, LayerMask.NameToLayer("PropOutline"));
        }

        // 3. 현재 Outline 객체 갱신
        oldNearestObject = nearestObj;
    }

    private void OnTriggerEnter(Collider other) {
        if (!isLocalPlayer) return;

        GameObject parentObj = PropUtil.GetPropParent(other.gameObject);

        if (parentObj == null) return;

        if (parentObj.layer != LayerMask.NameToLayer("Prop")) return;

        if (!hitObjs.Contains(parentObj)) hitObjs.Add(parentObj);
    }
    
    private void OnTriggerExit(Collider other) {
        if (!isLocalPlayer) return;

        GameObject parentObj = PropUtil.GetPropParent(other.gameObject);

        if (parentObj == null) return;
        
        hitObjs.Remove(parentObj);
    }

}
