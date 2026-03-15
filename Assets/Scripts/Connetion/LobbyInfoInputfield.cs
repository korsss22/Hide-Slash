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
        GetValues(out string lobbyName, out string password, out string maxPlayer, out LobbyType lobbyType);

        if (!CheckInputValid(lobbyName, maxPlayer)) return;
        if (!SteamClient.IsValid) {
            UIManager.PrintUI(DEBUG_TYPE.ERROR, "SteamClient is not valid...");
            return;
        }
        int.TryParse(maxPlayer, out int maxPlayer_int);
        NetworkController.Instance.CreateLobby(lobbyName, password, maxPlayer_int, lobbyType);
    }

    private void GetValues(out string lobbyName, out string password, out string maxPlayer, out LobbyType lobbyType) {
        lobbyName = lobbyNameField.text;
        password = passwordField.text;
        maxPlayer = maxPlayerDropdown.options[maxPlayerDropdown.value].text;
        
        Debug.Log(maxPlayer);

        Toggle activeToggle = lobbyTypeToggle.GetFirstActiveToggle();

        if (activeToggle == null) {
            Debug.Log("active Toggle is null");
            lobbyType = default;
            return;
        }

        Enum.TryParse(activeToggle.name, true, out lobbyType);
    }

    private bool CheckInputValid(string lobbyName, string maxPlayerIndex) {
        if (string.IsNullOrWhiteSpace(lobbyName)) {
            UIManager.PrintUI(DEBUG_TYPE.ERROR, "Check your lobby name is filled...");
            return false;
        }
        if (maxPlayerDropdown.options.Count == 0) {
            UIManager.PrintUI(DEBUG_TYPE.ERROR, "Check maxPlayer is Checked...");
            return false;
        }
        return true;
    }
}
