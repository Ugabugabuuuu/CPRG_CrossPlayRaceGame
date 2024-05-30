using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarGameLobby : MonoBehaviour
{
    public static CarGameLobby Instance { get; private set; }
    private Lobby joinLobby;
    private float heartbeatTimer;
    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler<OnLobbyListEventChangeArgs> OnLobbyListChanged;
    private float listLobbiesTimer;
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }
    private void Update()
    {
        HandleHeartBeat();
        HandleLobbyListChange();
    }
   private void HandleLobbyListChange()
    {
        if(joinLobby == null && AuthenticationService.Instance.IsSignedIn && SceneManager.GetActiveScene().name == Loader.Scene.Lobby.ToString())
        {
            listLobbiesTimer -= Time.deltaTime;
            if (listLobbiesTimer <= 0f)
            {
                listLobbiesTimer = 5f;
                ListLobbies();
            }
        }

    }
    private void HandleHeartBeat()
    {
        if(isLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0)
            {
                heartbeatTimer = 15;

                LobbyService.Instance.SendHeartbeatPingAsync(joinLobby.Id);
            }
        }
    }
    private bool isLobbyHost()
    {
        return joinLobby != null && joinLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0,10000).ToString()); //USE THIS ONLY FOR TESTING IN SAME MACHINE FUTURE ME
            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

    }
    public async void CreatLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerManager.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

           await  LobbyService.Instance.UpdateLobbyAsync(joinLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            MultiplayerManager.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CarSelectionScene);
        } catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }

    }
    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerManager.MAX_PLAYER_AMOUNT - 1);
            return allocation;
        }catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return default;
        }

    }
    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return joinCode;
        }catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return default;
        }

    }
    private async Task<JoinAllocation> JoinRelayWithCode(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return allocation;
        }catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return default;
        }
    }
    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelayWithCode(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

           // Debug.Log("client");
            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }

    }
    public Lobby GetLobby()
    {
        return joinLobby;
    }
    public async void JoinWithCode(string code)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);


            string relayJoinCode = joinLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelayWithCode(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerManager.Instance.StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }

    }
    public async void JoinWithId(string lobbyId)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            string relayJoinCode = joinLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelayWithCode(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }

    }
    public async void DeleteLobby()
    {
        if(joinLobby!=null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinLobby.Id);
                joinLobby = null;
            }catch(LobbyServiceException e)
            {
                Debug.LogError(e);
            }
  
        }
    }
    public async void LeaveLobby()
    {
        if (joinLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinLobby.Id, AuthenticationService.Instance.PlayerId);
                joinLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

    }
    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions query = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0", QueryFilter.OpOptions.GT)
            }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(query);
            OnLobbyListChanged?.Invoke(this, new OnLobbyListEventChangeArgs
            {
                lobbyList = queryResponse.Results
            });
        }
        catch(LobbyServiceException e)
        {
            Debug.LogError(e);
        }

    }
    public async void KickPlayerFromLobby(string playerId)
    {
        if (isLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

    }
}
public class OnLobbyListEventChangeArgs: EventArgs
{
    public List<Lobby> lobbyList = new List<Lobby>();
}
