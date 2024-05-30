using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;
    private AudioSource buttonClickAudioSource;
    private void Awake()
    {
        GameObject buttonClickObject = GameObject.FindWithTag("buttonClickSound");
        if (buttonClickObject != null)
        {

            buttonClickAudioSource = buttonClickObject.GetComponent<AudioSource>();
        }
        closeButton.onClick.AddListener(()=> {
            buttonClickAudioSource.Play();
            Hide();
            
        });
    }
    private void Start()
    {
        MultiplayerManager.Instance.OnFailedToJoinGame += MultiplayerManager_OnFailedToJoinGame;
        CarGameLobby.Instance.OnCreateLobbyStarted += Instance_OnCreateLobbyStarted;
        CarGameLobby.Instance.OnCreateLobbyFailed += Instance_OnCreateLobbyFailed;
        CarGameLobby.Instance.OnJoinStarted += Instance_OnJoinStarted;
        CarGameLobby.Instance.OnJoinFailed += Instance_OnJoinFailed;
        CarGameLobby.Instance.OnQuickJoinFailed += Instance_OnQuickJoinFailed;
        Hide();
    }

    private void Instance_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("No lobbies found");
    }

    private void Instance_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join lobby :(");
    }

    private void Instance_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining lobby");
    }

    private void Instance_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create lobby :(");
    }

    private void Instance_OnCreateLobbyStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating Lobby");
    }

    private void MultiplayerManager_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        if(NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Connection failed");
        }
        else 
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnFailedToJoinGame -= MultiplayerManager_OnFailedToJoinGame;
        CarGameLobby.Instance.OnCreateLobbyStarted -= Instance_OnCreateLobbyStarted;
        CarGameLobby.Instance.OnCreateLobbyFailed -= Instance_OnCreateLobbyFailed;
        CarGameLobby.Instance.OnJoinStarted -= Instance_OnJoinStarted;
        CarGameLobby.Instance.OnJoinFailed -= Instance_OnJoinFailed;
        CarGameLobby.Instance.OnQuickJoinFailed -= Instance_OnQuickJoinFailed;
    }
}
