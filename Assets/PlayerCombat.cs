using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private Player player;
    [SerializeField] private LayerMask pushableLayer;
    [SerializeField] private Vector3 checkBoxCenter;
    [SerializeField] private Vector3 checkBoxHalfExtents;
    [SerializeField] private float pushPower = 3f;

    public void RegisterEvent(Player player) {
        this.player = player;

        player.OnPushing += PushObject;
    }

    public void UnRegisterEvent() {
        player.OnPushing -= PushObject;
    }

    public void OnPush(InputAction.CallbackContext context) {
        if (context.started) player.OnPushingObject();
    }

    private void PushObject() {
        List<NetworkIdentity> identities = CheckPushObjectsIdentity();
        Debug.Log(identities.Count);
        player.CmdPush(identities, pushPower);
    }

    private List<NetworkIdentity> CheckPushObjectsIdentity() {
        Vector3 boxCenter = transform.TransformPoint(checkBoxCenter);
        Quaternion boxRotation = Quaternion.LookRotation(transform.forward, Vector3.up);

        Collider[] collideObjects = Physics.OverlapBox(
            boxCenter,
            checkBoxHalfExtents,
            boxRotation,
            pushableLayer,
            QueryTriggerInteraction.Ignore
        );

        List<NetworkIdentity> identities = new();

        foreach (Collider obj in collideObjects)
        {
            if (obj.TryGetComponent(out NetworkIdentity identity)) identities.Add(identity);
        }

        return identities;
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;

        Vector3 boxCenter = transform.TransformPoint(checkBoxCenter);
        Quaternion boxRotation = Quaternion.LookRotation(transform.forward, Vector3.up);

        Gizmos.matrix = Matrix4x4.TRS(boxCenter, boxRotation, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, checkBoxHalfExtents * 2f);

        Gizmos.matrix = oldMatrix;
    }
}
