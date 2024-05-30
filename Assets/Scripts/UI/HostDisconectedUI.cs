using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostDisconnectedUI : MonoBehaviour
{
    public static HostDisconnectedUI Instance { get; private set; }

    [SerializeField] private Button leaveButton;
    [SerializeField] private bool dontDestroyOnLoad = true;
    private bool hostDisconnected = false;
    private AudioSource buttonClickAudioSource;

    private void Awake()
    {
        GameObject buttonClickObject = GameObject.FindWithTag("buttonClickSound");

        if (buttonClickObject != null)
        {

            buttonClickAudioSource = buttonClickObject.GetComponent<AudioSource>();
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        leaveButton.onClick.AddListener(() => {
            buttonClickAudioSource.Play();
            Hide();
            Destroy(gameObject);
        });

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        Hide();
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            DestroyComponents();
            hostDisconnected = true;
            Loader.Load(Loader.Scene.Lobby);
        }
    }

    private void DestroyComponents()
    {
        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);
        if (MultiplayerManager.Instance != null)
            Destroy(MultiplayerManager.Instance.gameObject);
        if (CarGameLobby.Instance != null)
            Destroy(CarGameLobby.Instance.gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == Loader.Scene.Lobby.ToString() & hostDisconnected)
        {
            Show();
            hostDisconnected = false;
        }
    }


    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
