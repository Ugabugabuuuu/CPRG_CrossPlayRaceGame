using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class GameLobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private CreateLobbyUI createLobbyUI;
    [SerializeField] private Transform lobbyListScrollViewContainer;
    [SerializeField] private Transform lobbyTemplate;
    private AudioSource buttonClickAudioSource;

    private void Start()
    {
        playerNameInputField.text = MultiplayerManager.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string name) =>
        {
            MultiplayerManager.Instance.SetPlayerName(name);
        });
        CarGameLobby.Instance.OnLobbyListChanged += Instance_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void Instance_OnLobbyListChanged(object sender, OnLobbyListEventChangeArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void Awake()
    {
        GameObject buttonClickObject = GameObject.FindWithTag("buttonClickSound");

        if (buttonClickObject != null)
        {

            buttonClickAudioSource = buttonClickObject.GetComponent<AudioSource>();
        }

        lobbyTemplate.gameObject.SetActive(false);

        mainMenuButton.onClick.AddListener(() =>
        {
            buttonClickAudioSource.Play();
            CarGameLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.Menu);
        });
        createLobbyButton.onClick.AddListener(() =>
        {
            buttonClickAudioSource.Play();
            createLobbyUI.Show();
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            buttonClickAudioSource.Play();
            CarGameLobby.Instance.QuickJoin();
        });
        joinCodeButton.onClick.AddListener(() =>
        {
            buttonClickAudioSource.Play();
            CarGameLobby.Instance.JoinWithCode(codeInputField.text);
        });
    }
    private void UpdateLobbyList(List<Lobby> lobbies)
    {
        foreach(Transform t in lobbyListScrollViewContainer)
        {
            if (t == lobbyTemplate)
                continue;
            else
            {
                Destroy(t.gameObject);
            }
        }
        foreach(Lobby lobby in lobbies)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyListScrollViewContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyTemplateUI>().SetLobby(lobby);
        }
    }
    private void OnDestroy()
    {
        CarGameLobby.Instance.OnLobbyListChanged -= Instance_OnLobbyListChanged;
    }
}
