using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class InGameMenuUI : MonoBehaviour
{
    private Settings settings;
    private const string settingsKey = "GAME_SETTINGS";

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixer audioMixerMusic;
    [SerializeField] private Slider gameVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Button leaveButton;
    private AudioSource buttonClickAudioSource;
    private void Awake()
    {
        GameObject buttonClickObject = GameObject.FindWithTag("buttonClickSound");
        if (buttonClickObject != null)
        {

            buttonClickAudioSource = buttonClickObject.GetComponent<AudioSource>();
        }
        gameVolumeSlider.onValueChanged.AddListener(SetGameVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

        leaveButton.onClick.AddListener(() =>
        {
            buttonClickAudioSource.Play();
            CarGameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.Lobby);
        });
    }

    private void Start()
    {
        LoadSettings();
        ApplySettingsToUI();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(settingsKey))
        {
            string json = PlayerPrefs.GetString(settingsKey);
            settings = JsonUtility.FromJson<Settings>(json);
        }
        else
        {
            settings = new Settings();
        }

        audioMixer.SetFloat("volume", settings.volume);
        audioMixerMusic.SetFloat("musicVolume", settings.musicVolume);

        Application.targetFrameRate = settings.targetFPS;
    }

    private void ApplySettingsToUI()
    {
        gameVolumeSlider.value = settings.volume;
        musicVolumeSlider.value = settings.musicVolume;
    }
    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings);
        PlayerPrefs.SetString(settingsKey, json);
        PlayerPrefs.Save();
    }

    public void SetGameVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        settings.volume = volume;
        SaveSettings();
    }

    public void SetMusicVolume(float musicVolume)
    {
        audioMixerMusic.SetFloat("musicVolume", musicVolume);
        settings.musicVolume = musicVolume;
        SaveSettings();
    }
    public void SetFPS(int fps)
    {
        Application.targetFrameRate = fps;
        settings.targetFPS = fps;
        SaveSettings();
    }
}
