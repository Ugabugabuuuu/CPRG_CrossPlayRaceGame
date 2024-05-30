using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
  
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createPublicLobbyButton;
    [SerializeField] private Button createPrivateLobbyButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    private AudioSource buttonClickAudioSource;

    private void Start()
    {
        Hide();
    }
    private void Awake()
    {
        GameObject buttonClickObject = GameObject.FindWithTag("buttonClickSound");

        if (buttonClickObject != null)
        {

            buttonClickAudioSource = buttonClickObject.GetComponent<AudioSource>();
        }

        closeButton.onClick.AddListener(() =>
        {
            buttonClickAudioSource.Play();
            Hide();
        });
        createPublicLobbyButton.onClick.AddListener(() =>
        {
            buttonClickAudioSource.Play();
            CarGameLobby.Instance.CreatLobby(lobbyNameInputField.text, false);
        });
        createPrivateLobbyButton.onClick.AddListener(() =>
        {
            buttonClickAudioSource.Play();
            CarGameLobby.Instance.CreatLobby(lobbyNameInputField.text, true);
        });
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public String GetLobbyName()
    {
        return lobbyNameInputField.text.ToString();
    }
}
