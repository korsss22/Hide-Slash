using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.InputSystem;

public class PropTransformSystem : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PropDatabase objDatabase;
    private HoldInteractionUI holdUI;
    private GameObject transformedObj = null;
    public GameObject nearestObj = null;
    public event Action<GameObject> OnNearestObjectChanged;

    void Awake()
    {
        if (player == null) GetComponentInParent<Player>();
    }

    [ClientCallback]
    private void Start() {
        holdUI = UIUtils.GetHoldUI();    
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

    [ClientCallback]
    public void OnInteract(InputAction.CallbackContext context) {
        player.gameObject.TryGetComponent(out NetworkIdentity identity);
        
        if (!player.isLocalPlayer) {
            Debug.Log($"{player.name} is not localPlayer connId : {identity.connectionToClient.connectionId} from : {gameObject}");
            return;
        } 

        if (!player.IsTransformed()) {
            if (nearestObj == null) {
                return;
            } else {
                Debug.Log("Transforming...");
            }
        }

        if (context.started) holdUI.StartHold(OnCompleteHold);
        else if (context.performed) holdUI.CompleteHold();
        else if (context.canceled) holdUI.CancelHold();
    }

    [Client]
    private void OnCompleteHold()
    {
        if (!player.isLocalPlayer) {
            Debug.Log($"{player.name} is not localPlayer from : {player.gameObject}");
            return;
        } 

        if (!player.IsTransformed())
        {
            if (nearestObj == null) return;

            PropParent prop = nearestObj.GetComponent<PropParent>();
            if (prop == null) return;

            player.CmdRequestTransform(prop.propID);
        }
        else
        {
            player.CmdRequestTransform(-1);
        }
    }

    public void Apply(int prefabId) {
        player.Mesh.gameObject.SetActive(false);
        
        CreateTransformObj(prefabId);
    }

    public void Revert() {
        player.Mesh.gameObject.SetActive(true);

        Destroy(transformedObj);
    }

    private void CreateTransformObj(int prefabId) {
        PropIdentify prop = PropUtil.GetPropById(prefabId);

        if (prop == null) {
            Debug.LogError("May prefabId is not valid... PrefabId : "+prefabId);
            return;
        }

        GameObject transformObj = prop.propPrefab;

        transformedObj = Instantiate(transformObj, player.transform);
        
        PropUtil.SetPropLayerInChildren(transformedObj, LayerMask.NameToLayer("Prop"));

        PropUtil.SetAllChildColliders(transformedObj, false);
        transformedObj.transform.localPosition = Vector3.zero;
    }
}