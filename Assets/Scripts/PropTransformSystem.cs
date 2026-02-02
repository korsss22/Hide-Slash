using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

[RequireComponent(typeof(Collider))]
public class PropTransformSystem : NetworkBehaviour
{
    private Collider hitRange;
    [SerializeField] private List<GameObject> hitObjs = new();
    [SerializeField] private GameObject nearestObj = null;
    private GameObject transformObj = null;
    private Player player;

    private void Start() {
        StartCoroutine(LayerUpdate());
    }

    public void Init(Player player) { //Use this as Awake
        this.player = player;

        hitRange = GetComponent<Collider>();
    }

    public void Transform(InputValue value) {
        if (nearestObj == null) return;

        // Mesh mesh = nearestObj.GetComponent<MeshFilter>().mesh;
        // Material[] materials = nearestObj.GetComponent<MeshRenderer>().materials;

        // player.gameObject.GetComponentInChildren<MeshFilter>().mesh = mesh;
        // player.gameObject.GetComponentInChildren<MeshRenderer>().materials = materials;
        
        // player.meshFilter.gameObject.GetComponent<Collider>().enabled = false;

        // Collider source = nearestObj.GetComponent<Collider>();
        // Collider copied = ColliderUtil.CopyCollider(source, player.meshFilter.gameObject);
        player.isTransformed = true;
        player.Mesh.gameObject.SetActive(false);
        transformObj = Instantiate(nearestObj, player.transform);
        SetPropChild(transformObj, LayerMask.NameToLayer("Prop"));

        Collider[] cols = transformObj.GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
        {
            col.enabled = false;
        }
        transformObj.transform.localPosition = new Vector3(0,0,0);
    }  

    public void RevertTransform(InputValue value) {
        Destroy(transformObj);
        player.isTransformed = false;
        player.Mesh.gameObject.SetActive(true);
    }

    private IEnumerator LayerUpdate() {
        WaitForSeconds interval = new(0.1f);
        while (true) {
            if (hitObjs.Count == 0) {
                yield return interval;
                continue;
            }
            if (hitObjs.Count == 1) {
                SetPropChild(hitObjs[0], LayerMask.NameToLayer("PropOutline"));
                nearestObj =  hitObjs[0];
                yield return interval;
                continue;
            }
            
            nearestObj = GetNearestObj(hitObjs);
        
            foreach (GameObject obj in hitObjs)
            {
                if (obj.layer == LayerMask.NameToLayer("PropOutline")) {
                    SetPropChild(obj, LayerMask.NameToLayer("Prop"));
                }
            }
            SetPropChild(nearestObj, LayerMask.NameToLayer("PropOutline"));

            yield return interval;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!isLocalPlayer) return;

        GameObject parentObj = GetPropParent(other.gameObject);
        
        if (parentObj == null) return;
        
        if (parentObj.layer != LayerMask.NameToLayer("Prop")) return;

        if (!hitObjs.Contains(parentObj)) hitObjs.Add(parentObj);
    }
    
    private void OnTriggerExit(Collider other) {
        if (!isLocalPlayer) return;

        GameObject parentObj = GetPropParent(other.gameObject);

        if (parentObj == null) return;

        if (parentObj.layer == LayerMask.NameToLayer("PropOutline")) {
            nearestObj = null;
            SetPropChild(parentObj, LayerMask.NameToLayer("Prop"));
            hitObjs.Remove(parentObj);
        } // when its nearest obj

        if (parentObj.layer == LayerMask.NameToLayer("Prop")) {
            hitObjs.Remove(parentObj);
        } // when its candidates
    
        if (hitObjs.Count == 0) nearestObj = null;
    }

    private GameObject GetNearestObj(List<GameObject> objs)
    {
        if (objs.Count == 0) return null;
        if (objs.Count == 1) return objs[0];

        float minDis = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject obj in objs)
        {
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0) continue;

            foreach (Collider col in colliders)
            {   
                if (col is MeshCollider mesh) mesh.convex = true;
                Vector3 closestPoint = col.ClosestPoint(transform.position);
                float nowDis = Vector3.Distance(transform.position, closestPoint);

                if (nowDis < minDis)
                {
                    nearest = obj;
                    minDis = nowDis;
                }
                if (col is MeshCollider mesh2) mesh2.convex = false;
            }
        }

        return nearest;
    }

    private GameObject GetPropParent(GameObject obj) {
        // if (child.transform.parent == null) return null; 
        // if (child.transform.parent.gameObject.layer != layer) return child;
        // return GetPropParent(child.transform.parent.gameObject, layer); 
        PropParent parent = obj.GetComponentInParent<PropParent>();
        return parent ? parent.gameObject : null;
    }

    private void SetPropChild(GameObject parent, LayerMask layer) {
        parent.layer = layer;
        
        foreach (Transform child in parent.transform)
        {
            SetPropChild(child.gameObject, layer);
        }
    }
}
