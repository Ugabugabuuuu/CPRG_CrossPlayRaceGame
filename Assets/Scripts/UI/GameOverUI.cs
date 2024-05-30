using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] playerList = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] playerTimeList = new TextMeshProUGUI[4];
    [SerializeField] private TextMeshProUGUI[] placeTextList = new TextMeshProUGUI[4];
    [SerializeField] private Button leaveButton;
    List<RaceData> raceData = new List<RaceData>();
    Dictionary<int, RaceData> dictRaceData= new Dictionary<int, RaceData>();
    int counter;
    private bool isExecuted;
    private AudioSource buttonClickAudioSource;
    private void Start()
    {
        isExecuted = false;
        counter = 0;
        DisableAllFields();
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        GameManager.Instance.OnPlayerDataNetworkListChanged += Instance_OnPlayerDataNetworkListChanged;
        Hide();
    }
    private void Awake()
    {
        GameObject buttonClickObject = GameObject.FindWithTag("buttonClickSound");

        if (buttonClickObject != null)
        {

            buttonClickAudioSource = buttonClickObject.GetComponent<AudioSource>();
        }
        leaveButton.onClick.AddListener(()=>
        {
            buttonClickAudioSource.Play();  
            CarGameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.Menu);
        });
    }

    private void Instance_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        foreach (var player in GameManager.Instance.playerdataList)
        {
            counter++;
            
            ShowSpecific(placeTextList[player.finishPlace].gameObject, playerList[player.finishPlace].gameObject, playerTimeList[player.finishPlace].gameObject);

            int minutes = (int)player.finishTime / 60;
            int seconds = (int)player.finishTime % 60;

            playerList[player.finishPlace].text = player.playerName.ToString();
            playerTimeList[player.finishPlace].text = minutes.ToString() + ":" + ((seconds < 10) ? ("0") : ("")) + seconds.ToString();
            dictRaceData[player.finishPlace] = (new RaceData(player.finishPlace, player.playerName.ToString(), player.finishTime));
        }
        if(!isExecuted && GameManager.Instance.playerdataList.Count == MultiplayerManager.Instance.playerDataNetworkList.Count)
        {
            SaveGameData();

            isExecuted = true;
        }

    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();
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
    private void ShowSpecific(GameObject a, GameObject b, GameObject c)
    {
        a.SetActive(true);
        b.SetActive(true);
        c.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void DisableAllFields()
    {
        for(var i = 0; i<4; i++) 
        {
            playerList[i].gameObject.SetActive(false);
            playerTimeList[i].gameObject.SetActive(false);
            placeTextList[i].gameObject.SetActive(false);
        }
    }
    private void SaveGameData()
    {
        string lobbyName = MultiplayerManager.Instance.GetLobbyName();
        int numberOfPlayers = GameManager.Instance.playerdataList.Count;
        foreach (var rd in dictRaceData.Values)
        {
            raceData.Add(rd);
        }
        Debug.Log(lobbyName);
        GameData gameData = new GameData()
        {
            date = DateTime.Now.ToString("yyyy-MM-dd"),

            lobbyName = lobbyName,
            numberOfPlayers = numberOfPlayers,
            raceDataList = raceData
        };

        AllGameData allGameData = LoadAllGameData();
        allGameData.allGames.Add(gameData);
        string jsonData = JsonUtility.ToJson(allGameData);
        PlayerPrefs.SetString("AllGameData", jsonData);
        PlayerPrefs.Save();
    }

    private AllGameData LoadAllGameData()
    {
        string jsonData = PlayerPrefs.GetString("AllGameData");
        if (!string.IsNullOrEmpty(jsonData))
        {
            return JsonUtility.FromJson<AllGameData>(jsonData);
        }
        return new AllGameData();
    }
}
