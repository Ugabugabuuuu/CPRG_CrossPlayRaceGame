using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private Transform gameDataListScrollViewContainer;
    [SerializeField] private Transform raceDataContainer;
    [SerializeField] private GameObject gameDataTemplate;
    [SerializeField] private Transform raceDataTemplate;
    [SerializeField] private Transform gameDataGameObject;
    [SerializeField] private TextMeshProUGUI noDataText;
    [SerializeField] private GameObject raceDataGameobject;
    [SerializeField] private Button buttonToHide;
    [SerializeField] private Button closeRaceDataWindow;
    [SerializeField] private Button deleteGameDataButton;
    [SerializeField] private GameObject exitButtonGameObject;
    private AudioSource buttonClickAudioSource;
    void Start()
    {
        foreach (Transform t in gameDataListScrollViewContainer)
        {
            if (t == gameDataTemplate)
                continue;
            else
            {
                Destroy(t.gameObject);
            }
        }

        gameDataGameObject.gameObject.SetActive(true);
        noDataText.gameObject.SetActive(false);
        AllGameData allGameData = LoadAllGameData();

        if (allGameData.allGames.Count == 0)
        {
            noDataText.gameObject.SetActive(true);
        }
        else
        {
            foreach (GameData gameData in allGameData.allGames)
            {
                CreateGameDataItem(gameData);
            }
        }
    }
    private void Awake()
    {
        GameObject buttonClickObject = GameObject.FindWithTag("buttonClickSound");
        if (buttonClickObject != null)
        {

            buttonClickAudioSource = buttonClickObject.GetComponent<AudioSource>();
        }

        closeRaceDataWindow.onClick.AddListener(()=>
        {
            buttonClickAudioSource.Play();
            exitButtonGameObject.SetActive(true);
        });
        buttonToHide.onClick.AddListener(() =>
        {
            buttonClickAudioSource.Play();
        });
        deleteGameDataButton.onClick.AddListener(()=>
        {
            buttonClickAudioSource.Play();
            ClearRaceData();
            foreach (Transform t in gameDataListScrollViewContainer)
            {
                if (t == gameDataTemplate)
                    continue;
                else
                {
                    Destroy(t.gameObject);
                }
            }
            noDataText.gameObject.SetActive(true);
        });
    }
    private static void ClearRaceData()
    {
        PlayerPrefs.DeleteKey("AllGameData");
        PlayerPrefs.Save();
    }
    private void CreateGameDataItem(GameData gameData)
    {
        GameObject gameDataItem = Instantiate(gameDataTemplate, gameDataListScrollViewContainer);
        gameDataItem.transform.Find("dateText").GetComponent<TextMeshProUGUI>().text = gameData.date;
        gameDataItem.transform.Find("lobbyNameText").GetComponent<TextMeshProUGUI>().text = gameData.lobbyName.ToString();
        gameDataItem.transform.Find("playerText").GetComponent<TextMeshProUGUI>().text = gameData.numberOfPlayers.ToString();

        Button button = gameDataItem.GetComponent<Button>();
        button.onClick.AddListener(() => ShowRaceData(gameData.raceDataList));
        gameDataItem.SetActive(true);
        gameDataGameObject.gameObject.SetActive(true);
    }



    private void ShowRaceData(List<RaceData> raceDataList)
    {
        foreach (Transform t in raceDataContainer)
        {
            if (t == raceDataTemplate)
                continue;
            else
            {
                Destroy(t.gameObject);
            }
        }

        float totalHeight = 0;
        float itemHeight = raceDataTemplate.GetComponent<RectTransform>().rect.height;

        foreach (RaceData data in raceDataList)
        {
            Transform raceData = Instantiate(raceDataTemplate, raceDataContainer);
            raceData.transform.Find("placeText").GetComponent<TextMeshProUGUI>().text = FormatPlace(data.place+1);
            raceData.transform.Find("nameText").GetComponent<TextMeshProUGUI>().text = data.name;
            raceData.transform.Find("timeText").GetComponent<TextMeshProUGUI>().text = FormatTime(data.time);

            raceData.gameObject.SetActive(true);
            
            totalHeight += itemHeight;
        }
        raceDataGameobject.gameObject.SetActive(true);
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

    private string FormatPlace(int place)
    {
        switch (place % 10)
        {
            case 1: return place % 100 == 11 ? place + "th" : place + "st";
            case 2: return place % 100 == 12 ? place + "th" : place + "nd";
            case 3: return place % 100 == 13 ? place + "th" : place + "rd";
            default: return place + "th";
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = (int)timeInSeconds / 60;
        int seconds = (int)timeInSeconds % 60;
        return string.Format("{0}:{1:00}", minutes, seconds);
    }
}