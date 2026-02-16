using UnityEngine;

public class PanelUtils : MonoBehaviour
{
    public void EnterPanel(GameObject panel) {
        panel.SetActive(true);
    }

    public void ExitPanel(GameObject panel) {
        panel.SetActive(false);
    }

    public void TogglePanel(GameObject panel) {
        panel.SetActive(!panel.activeSelf);
    }
}
