using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Button createGameButton;
    [SerializeField] Button joinGameButton;

    private void Awake()
    {
        createGameButton.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.StartHost();
          //  Debug.Log("host");
            Loader.LoadNetwork(Loader.Scene.CarSelectionScene);
        });
        joinGameButton.onClick.AddListener(() =>
        {
           // Debug.Log("client");
            MultiplayerManager.Instance.StartClient();
        });
    }

}
