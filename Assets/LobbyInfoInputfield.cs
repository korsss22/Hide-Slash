using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfoInputfield : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_Dropdown maxPlayerDropdown;
    [SerializeField] private ToggleGroup lobbyTypeToggle;

    public void CreateLobby() {   
        GetValues(out string lobbyName, out string password, out int maxPlayerIndex, out LobbyType lobbyType);

        if (!CheckInputValid(lobbyName, maxPlayerIndex)) return;
        if (!SteamClient.IsValid) {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "SteamClient is not valid...");
            return;
        }
        NetworkController.Instance.CreateLobby(lobbyName, password, maxPlayerIndex, lobbyType);
    }

    private void GetValues(out string lobbyName, out string password, out int maxPlayerIndex, out LobbyType lobbyType) {
        lobbyName = lobbyNameField.text;
        password = passwordField.text;
        maxPlayerIndex = maxPlayerDropdown.value;

        Toggle activeToggle = lobbyTypeToggle.GetFirstActiveToggle();

        if (activeToggle == null) {
            Debug.Log("active Toggle is null");
            lobbyType = default;
            return;
        }

        Enum.TryParse(activeToggle.name, true, out lobbyType);
    }

    private bool CheckInputValid(string lobbyName, int maxPlayerIndex) {
        if (string.IsNullOrWhiteSpace(lobbyName)) {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "Check your lobby name is filled...");
            return false;
        }
        if (maxPlayerDropdown.options.Count == 0) {
            UIUtils.PrintUI(DEBUG_TYPE.ERROR, "Check maxPlayer is Checked...");
            return false;
        }
        return true;
    }
}
