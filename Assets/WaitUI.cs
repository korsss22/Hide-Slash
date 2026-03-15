using UnityEngine;
using UnityEngine.UI;

public class WaitUI : MonoBehaviour
{
    [SerializeField] private Text infoText;
    [SerializeField] private Text connection;
    [SerializeField] private Text countdown;

    public void UpdateInfoText(string info) {
        infoText.text = info;
    }

    public void UpdateConnections(string nowPlayer, string maxPlayer) {
        connection.text = $"{nowPlayer} / {maxPlayer}";
    }

    public void UpdateCountdown(double remaingTime) {
        countdown.text = remaingTime.ToString();
    }

    public void ShowCountdown() {
        countdown.gameObject.SetActive(true);
    }

    public void HideCountdown() {
        countdown.gameObject.SetActive(false);
    }
}
