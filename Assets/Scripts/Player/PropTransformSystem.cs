using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PropTransformSystem : NetworkBehaviour
{
    [SerializeField] private Player player;
    private GameObject transformedObj = null;
    public GameObject nearestObj = null;
    public event Action<GameObject> OnNearestObjectChanged;

    public void Transform() { //Its local yet
        player.Mesh.gameObject.SetActive(false);
        transformedObj = Instantiate(nearestObj, player.transform);
        
        PropUtil.SetPropLayerInChildren(transformedObj, LayerMask.NameToLayer("Prop"));

        Collider[] cols = transformedObj.GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
        {
            col.enabled = false;
        }
        transformedObj.transform.localPosition = new Vector3(0,0,0);
        if (isServer) NetworkServer.Spawn(transformedObj);
    }
    public void RevertTransform() {
        Destroy(transformedObj);
        player.isTransformed = false;
        player.Mesh.gameObject.SetActive(true);
    }

    [Command]
    public void CmdTransform() {
        Debug.Log("Transform");
        player.isTransformed = true;
        Transform();
    }



    public GameObject GetNearestObj(List<GameObject> objs)
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
                Vector3 closestPoint = col.ClosestPoint(transform.position);
                float nowDis = Vector3.Distance(transform.position, closestPoint);

                if (nowDis < minDis)
                {
                    nearest = obj;
                    minDis = nowDis;
                }
            }
        }

        return nearest;
    }

    public void UpdateNearestObj(List<GameObject> hitObjs) {
        if (hitObjs.Count == 0) {
            nearestObj = null;
            OnNearestObjectChanged?.Invoke(nearestObj);
            return;
        }
        if (hitObjs.Count == 1) {
            nearestObj = hitObjs[0];
            OnNearestObjectChanged?.Invoke(nearestObj);
            return;
        }
        GameObject newNearestObj = GetNearestObj(hitObjs);
        nearestObj = newNearestObj;
        OnNearestObjectChanged?.Invoke(newNearestObj);
        return;
    }
}