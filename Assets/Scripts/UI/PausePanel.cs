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
        UIManager.CloseUIWithLock(gameObject);
    }

    void OnExitButtonClicked() {
        Debug.Log("Game Off!");
        Application.Quit();
    }
}
