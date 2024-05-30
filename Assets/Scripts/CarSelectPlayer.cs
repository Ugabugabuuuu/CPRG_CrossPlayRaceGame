using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CarSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyTextField;
    [SerializeField] private PlayerColors playerColors;
    [SerializeField] private  Button kickPlayerButton;
    [SerializeField] private TextMeshPro playerName;
    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerManager_OnPlayerDataNetworkListChanged;
        CarSelectReady.Instance.OnReadyChanged += CarSelectReady_OnReadyChanged;
        showKickButton();
        UpdatePlayer();
    }
    private void showKickButton()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            kickPlayerButton.gameObject.SetActive(true);
        }
        else 
            kickPlayerButton.gameObject.SetActive(false);
    }
    private void Awake()
    {
        kickPlayerButton.onClick.AddListener(() =>
        {
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            CarGameLobby.Instance.KickPlayerFromLobby(playerData.playerId.ToString());
            MultiplayerManager.Instance.KickPlayer(playerData.clientId);
        });
    }
    private void CarSelectReady_OnReadyChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }
    private void MultiplayerManager_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }
    private void UpdatePlayer()
    {
        if (MultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyTextField.SetActive(CarSelectReady.Instance.IsPlayerReady(playerData.clientId));
            playerColors.SetPlayerCarColor(MultiplayerManager.Instance.GetPlayerColor(playerData.colorId));
            playerName.text = playerData.playerName.ToString();
        }
        else
        {
            Hide();
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
}
