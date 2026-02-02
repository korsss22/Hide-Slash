using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CollisionChecker : MonoBehaviour
{
    private Player player;

    public void Init(Player player) {
        this.player = player;
    }

    private void Update() {
        Debug.Log(player.isCollision);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Ground")) {
            player.isCollision = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Ground")) {
            player.isCollision = false;
        }
    }
}
