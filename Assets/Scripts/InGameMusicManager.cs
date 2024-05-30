using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMusicManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource; 
    [SerializeField] List<AudioClip> gameStartedClips; 
    [SerializeField] AudioClip gameEndClip; 
    [SerializeField] List<AudioClip> countdownClips;

    public event EventHandler OnGameStarted; 
    public event EventHandler OnGameEnded; 

    private int countdownClipIndex; 
    private int gameStartedClipIndex; 
    private bool gameEnded = false;
    public static InGameMusicManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        countdownClipIndex = 0;
        gameStartedClipIndex = 0;
        musicSource.Stop(); 
        OnGameStarted += InGameMusicManager_OnGameStarted; 
        OnGameEnded += InGameMusicManager_OnGameEnded; 

        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged; 
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            StartCoroutine(CycleCountdownClips()); 
        }
    }

    private IEnumerator CycleCountdownClips()
    {

        while (countdownClipIndex < countdownClips.Count)
        {
            musicSource.clip = countdownClips[countdownClipIndex];
            musicSource.Play();

            yield return new WaitForSeconds(0.9f);

            countdownClipIndex++;
        }


        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    private void InGameMusicManager_OnGameStarted(object sender, EventArgs e)
    {
        StartCoroutine(PlayGameStartedClips()); 
    }

    private IEnumerator PlayGameStartedClips()
    {

        while (!gameEnded) 
        {
            if (gameStartedClipIndex >= gameStartedClips.Count)
                gameStartedClipIndex = 0;

            musicSource.clip = gameStartedClips[gameStartedClipIndex];
            musicSource.Play();

          
            while (musicSource.isPlaying)
            {
                yield return null; 
            }

            gameStartedClipIndex++; 
        }
    }

    private void InGameMusicManager_OnGameEnded(object sender, EventArgs e)
    {
        gameEnded = true;
        if (gameEndClip != null)
        {
            musicSource.clip = gameEndClip; 
            musicSource.Play();
        }
    }

    public void InvokeOnGameEnded()
    {
        OnGameEnded?.Invoke(this, EventArgs.Empty); 
    }
}
