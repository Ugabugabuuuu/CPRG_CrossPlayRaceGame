using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Unity.Netcode;
using UnityEngine;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }



    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalPlayerReadyChanged;
    public event EventHandler OnLocalPlayerFinish;
    public event EventHandler OnFinished;
    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    [SerializeField] private Transform playerPrefab;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private float localTimer = 0f;
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerCrossedFinishLineDictionary;
    public Dictionary<ulong, PlayerRaceData> playerRaceData;
    private bool isPlayerFinished;
    private int placeCounter;
    private NetworkVariable<int> place;
    public NetworkList<PlayerData> playerdataList;
    public event EventHandler OnPlayerDataNetworkListChanged;


    private void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerCrossedFinishLineDictionary = new Dictionary<ulong, bool>();
        playerRaceData = new Dictionary<ulong, PlayerRaceData>();
        place = new NetworkVariable<int>(0);
        playerdataList = new NetworkList<PlayerData> {};
        playerdataList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        FinishLineTriggerCheck.Instance.OnLocalPlayerFinsihedChanged += LocalInstance_OnLocalPlayerFinsihedChanged;
        placeCounter = 0;
    }
    public void InvokeOnFinished()
    {
        OnFinished?.Invoke(this, EventArgs.Empty);
    }
    private void OnClientConnected(ulong clientId)
    {
        if (!playerCrossedFinishLineDictionary.ContainsKey(clientId))
        {
            playerCrossedFinishLineDictionary.Add(clientId, false);
        }
        if (!playerCrossedFinishLineDictionary.ContainsKey(clientId))
        {
            PlayerRaceData r;
            r.time = 0f;
            r.place = -1;
            
            playerRaceData.Add(clientId, r);
        }

    }
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            OnClientConnected(clientId);
        }
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();
        }
    }
    private void LocalInstance_OnLocalPlayerFinsihedChanged(object sender, EventArgs e)
    {
        MyEventArgs myEventArgs = e as MyEventArgs;
        if (state.Value == State.GamePlaying)
        {
            isPlayerFinished = true;
            OnLocalPlayerFinish?.Invoke(this, EventArgs.Empty);
            placeCounter++;
            SetPlayerFinishedServerRpc(myEventArgs.ID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {

        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {

            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            state.Value = State.CountdownToStart;
        }
    }
      [ServerRpc(RequireOwnership = false)]
      private void SetPlayerFinishedServerRpc(ulong id, ServerRpcParams serverRpcParams = default)
      {
        PlayerData playerData = new PlayerData();

          playerCrossedFinishLineDictionary[id] = true;

        bool allClientsFinished = true;
        PlayerData tmp = MultiplayerManager.Instance.playerDataNetworkList[MultiplayerManager.Instance.GetPlayerDataIndexFromClientId(id)];
        int s = 0;
        playerData.clientId = id;
        playerData.finishTime = localTimer;
        playerData.finishPlace = place.Value;
        playerData.playerName = tmp.playerName;

        playerdataList.Add(playerData);
        place.Value++;
        PlayerRaceData raceData = new PlayerRaceData(place.Value, localTimer);
        playerRaceData[id] = raceData;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
          {

              if (!playerCrossedFinishLineDictionary.ContainsKey(clientId) || !playerCrossedFinishLineDictionary[clientId])
              {
                  allClientsFinished = false;

                break;
              }
            else
            {
                s++;
            }
        }
        GetAllConectedClients();

          if (s==NetworkManager.Singleton.ConnectedClientsIds.Count)
          {
              state.Value = State.GameOver;
          }
      }
    private void GetAllConectedClients ()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log(clientId + " all clients ids "+ playerCrossedFinishLineDictionary[clientId]);
        }
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        
        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = 0;
                }
                break;
            case State.GamePlaying:
                
                gamePlayingTimer.Value += Time.deltaTime;
                localTimer += Time.deltaTime;
                break;
            case State.GameOver:
                break;
        }
    }
    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state.Value == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }

     public bool IsWaitingToStart()
     {
          return state.Value == State.WaitingToStart;
      }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public string GetGamePlayingTimerNormalized()
    {
        int minutes = (int)gamePlayingTimer.Value / 60;
        int seconds = (int)gamePlayingTimer.Value % 60;
        return minutes.ToString() + ":" + ((seconds < 10) ? ("0") : ("")) + seconds.ToString();
    }
}
public struct PlayerRaceData
{
    public int place;
    public float time;

    public PlayerRaceData(int finishedPlace,  float t)
    {
        place = finishedPlace;
        time = t;
    }
}



