using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    private Settings settings;
    private const string settingsKey = "GAME_SETTINGS";

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixer audioMixerMusic;
    [SerializeField] private TMP_Dropdown resolutionDropDown;
    [SerializeField] private Slider gameVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown fpsDropdown;
    [SerializeField] private GameObject resulutionContainer;

    private Resolution[] resolutions;
    public static SettingsMenuController LocalInstance { get; private set; }
    private void Awake()
    {
        LocalInstance = this;

        if (Application.platform != RuntimePlatform.Android)
        {
            resolutions = Screen.resolutions;

            InitializeResolutionDropdown();

            resolutionDropDown.onValueChanged.AddListener(SetResolution);
        }
        else
        {
            resulutionContainer.SetActive(false);
        }

        gameVolumeSlider.onValueChanged.AddListener(SetGameVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
        qualityDropdown.onValueChanged.AddListener(SetGraphicsQuality);
        fpsDropdown.onValueChanged.AddListener(OnFPSDropdownChange);

        InitializeFPSDropdown();
    }

    private void InitializeResolutionDropdown()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            List<string> resolutionOptions = new List<string>();
            foreach (Resolution res in resolutions)
            {
                resolutionOptions.Add($"{res.width}x{res.height}");
            }
            resolutionDropDown.ClearOptions();
            resolutionDropDown.AddOptions(resolutionOptions);
        }
    }

    private void InitializeFPSDropdown()
    {
        List<string> fpsOptions = new List<string>();
        foreach (FPSList fps in System.Enum.GetValues(typeof(FPSList)))
        {
            fpsOptions.Add(((int)fps).ToString());
        }
        fpsDropdown.ClearOptions();
        fpsDropdown.AddOptions(fpsOptions);
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

        QualitySettings.SetQualityLevel(settings.qualityIndex);
        Screen.fullScreen = settings.isFullScreen;

        if (Application.platform != RuntimePlatform.Android)
        {
            int resolutionIndex = Mathf.Clamp(settings.resolutionIndex, 0, resolutions.Length - 1);
            resolutionDropDown.value = resolutionIndex;
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        Application.targetFrameRate = settings.targetFPS;
    }

    private void ApplySettingsToUI()
    {
        gameVolumeSlider.value = settings.volume;
        musicVolumeSlider.value = settings.musicVolume;
        qualityDropdown.value = settings.qualityIndex;
        fullScreenToggle.isOn = settings.isFullScreen;

        int fpsIndex = System.Array.IndexOf(System.Enum.GetValues(typeof(FPSList)), (FPSList)settings.targetFPS);
        fpsDropdown.value = fpsIndex;
    }

    private void OnFPSDropdownChange(int index)
    {
        FPSList selectedFPS = (FPSList)System.Enum.GetValues(typeof(FPSList)).GetValue(index);
        SetFPS((int)selectedFPS);
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

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        settings.qualityIndex = qualityIndex;
        SaveSettings();
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        settings.isFullScreen = isFullScreen;
        SaveSettings();
    }

    public void SetResolution(int resolutionIndex)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        settings.resolutionIndex = resolutionIndex;
        SaveSettings();
    }

    public void SetFPS(int fps)
    {
        Application.targetFrameRate = fps;
        settings.targetFPS = fps;
        SaveSettings();
    }

    // Test helpers

    public float GetGameVolume()
    {
        audioMixer.GetFloat("volume", out float value);
        return value;
    }

    public float GetMusicVolume()
    {
        audioMixerMusic.GetFloat("musicVolume", out float value);
        return value;
    }

    public bool GetFullScreen()
    {
        return settings.isFullScreen;
    }
}

public enum FPSList
{
    FPS_30 = 30,
    FPS_60 = 60,
    FPS_120 = 120
}
