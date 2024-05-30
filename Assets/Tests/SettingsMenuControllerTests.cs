using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SettingsMenuControllerTests
{
    private readonly string volumeString = "volume";
    private readonly string musicVolumeString = "musicVolume";

    GameObject obj;
    SettingsMenuController settingsController;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MenuTest", LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            yield return null; 
        }
        obj = GameObject.Find("SetupSettings");
        settingsController = obj.GetComponent<SettingsMenuController>();
        Clear();
    }

    [TearDown]
    public void TearDown()
    {
        Clear();
    }

    [Test]
    public void AudioMixer_SetsVolume()
    {
        obj = GameObject.Find("SetupSettings");
        settingsController = obj.GetComponent<SettingsMenuController>();

        settingsController.SetGameVolume(5);
        Assert.AreEqual(5, settingsController.GetGameVolume());
    }

    [Test]
    public void AudioMixer_SetMusicVolume()
    {
        settingsController.SetMusicVolume(5);
        Assert.AreEqual(5, settingsController.GetMusicVolume());
    }

    [Test]
    public void SetGraphicsQuality_SetsIndex()
    {
        settingsController.SetGraphicsQuality(1);
        Assert.AreEqual(1, QualitySettings.GetQualityLevel());
    }

    [Test]
    public void SetFullScreen_Sets()
    {
        settingsController.SetFullScreen(true);
        Assert.True(settingsController.GetFullScreen());
    }

    [Test]
    public void SetResolution_KeepsResolutionOnStandalone()
    {
        Resolution cur = Screen.currentResolution;

        settingsController.SetResolution(1);

        Resolution res = Screen.currentResolution;

        Assert.AreEqual(cur, res);
    }

    [Test]
    public void SetFPS_Sets()
    {
        settingsController.SetFPS(10);
        Assert.AreEqual(10, Application.targetFrameRate);
    }

    void Clear()
    {
        settingsController.SetMusicVolume(0);
        settingsController.SetGameVolume(0);
        QualitySettings.SetQualityLevel(0);
        Screen.fullScreen = false;
        Screen.SetResolution(0, 0, false);
        Application.targetFrameRate = 0;
        PlayerPrefs.Save();
    }
}
