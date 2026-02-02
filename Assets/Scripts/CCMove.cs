using Mirror.BouncyCastle.Asn1.X509;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CCMove : MonoBehaviour
{
    private Camera cam;
    private CharacterController cc;
    private Vector2 inputVec;

    private void Awake() {
        cc = GetComponent<CharacterController>();
    }

    private void Start() { 
        cam = Camera.main;
    }

    void OnMove(InputValue value) {
        inputVec = value.Get<Vector2>();
    }
    
    private void Update() {
        if (inputVec == Vector2.zero) return;

        Vector3 moveVec = cam.transform.forward * inputVec.y + cam.transform.right * inputVec.x;

        cc.SimpleMove(moveVec * 12f);
    }
}
