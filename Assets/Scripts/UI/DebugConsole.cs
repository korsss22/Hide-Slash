using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Text content;
    [SerializeField] private Button okButton;
    public Queue<DebugData> debugQueue = new();
    public bool isShowing = false;

    private void Awake() {
        okButton.onClick.AddListener(Show);
    }

    public void Show() {
        if (debugQueue.Count == 0) {
            gameObject.SetActive(false);
            isShowing = false;
            GameManager.Instance.ExitLockState();
            return;
        }
        GameManager.Instance.EnterLockState(LockState.UNLOCKED);
        isShowing = true;

        DebugData data = debugQueue.Dequeue();
        
        title.text = data.title;
        content.text = data.content;

        gameObject.SetActive(true);
    }
}
