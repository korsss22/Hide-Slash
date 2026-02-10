using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.InputSystem;

public class PropTransformSystem : NetworkBehaviour
{
    [SerializeField] private Player player;
    private HoldInteractionUI holdUI;
    private GameObject transformedObj = null;
    public GameObject nearestObj = null;
    public event Action<GameObject> OnNearestObjectChanged;

    void Awake()
    {
        holdUI = UIUtils.GetHoldUI(OnCompleteHold);
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

    public void OnInteract(InputAction.CallbackContext context) {
        if (nearestObj == null) return;
        
        if (context.started) holdUI.StartHold();
        else if (context.performed) holdUI.CompleteHold();
        else if (context.canceled) holdUI.CancelHold();
    }

    public void OnCompleteHold()
    {
        Debug.Log("Hold Interaction Complete. Nearest Object Id: " + nearestObj.GetComponent<PropParent>().propID);
    }
}