using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private Button resume;
    [SerializeField] private Button exit;

    private void Awake() {
        resume.onClick.AddListener(OnResumeButtonClicked);
        exit.onClick.AddListener(OnExitButtonClicked);
    }

    void OnResumeButtonClicked() {
        UIUtils.CloseUI(gameObject);
    }

    void OnExitButtonClicked() {
        Debug.Log("Game Off!");
        Application.Quit();
    }
}
