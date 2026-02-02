using UnityEngine;

public class UIEvents : MonoBehaviour
{
    public void HideUI(GameObject gameObject) {
        gameObject.SetActive(false);
    }

    public void ToggleUI(GameObject gameObject) {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
