using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Lobbies.Models;
using System.Runtime.CompilerServices;
using System;

public class CarSelectUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI joinCodeText;
    private string lobbyName;
    private AudioSource buttonClickAudioSource;
    public static CarSelectUI LocalInstance { get; private set; }
    private void Awake()
    {
        GameObject buttonClickObject = GameObject.FindWithTag("buttonClickSound");

        if (buttonClickObject != null)
        {

            buttonClickAudioSource = buttonClickObject.GetComponent<AudioSource>();
        }
        LocalInstance = this;
        mainMenuButton.onClick.AddListener(() =>
        {
            buttonClickAudioSource.Play();
            CarGameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
           Loader.Load(Loader.Scene.Menu);
        });
        readyButton.onClick.AddListener(() =>
        {
            buttonClickAudioSource.Play();
            CarSelectReady.Instance.SetPlayerReady();
        });
    }

    private void Start()
    {
       Lobby lobby = CarGameLobby.Instance.GetLobby();

        lobbyNameText.text = "Lobby name: " + lobby.Name;
        joinCodeText.text = "Join code: " + lobby.LobbyCode;
        lobbyName = lobby.Name;
        MultiplayerManager.Instance.Invoke_onCarSelectUILoaded();
    }
    public string GetLobbyName()
    {
        return lobbyName;
    }
}
