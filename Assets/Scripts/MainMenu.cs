using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject OptionsMenu;
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button statusButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;
    private AudioSource buttonClickAudioSource;

    private void Awake()
    {
        GameObject buttonClickObject = GameObject.FindWithTag("buttonClickSound");

        if (buttonClickObject != null)
        {
            buttonClickAudioSource = buttonClickObject.GetComponent<AudioSource>();
        }

        playButton.onClick.AddListener(() =>
        {
            if (buttonClickAudioSource != null) buttonClickAudioSource.Play();
            Loader.Load(Loader.Scene.Lobby);
        });
        optionsButton.onClick.AddListener(() =>
        {
            if (buttonClickAudioSource != null) buttonClickAudioSource.Play();
        });
        statusButton.onClick.AddListener(() =>
        {
            if (buttonClickAudioSource != null) buttonClickAudioSource.Play();
        });
        exitButton.onClick.AddListener(() =>
        {
            if (buttonClickAudioSource != null) buttonClickAudioSource.Play();
        });
        backButton.onClick.AddListener(() =>
        {
            if (buttonClickAudioSource != null) buttonClickAudioSource.Play();
        });
    }

    public void PlayGame()
    {
        mainMenu.SetActive(false);
        OptionsMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
