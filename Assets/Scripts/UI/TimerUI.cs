using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button repawnButton;
    private void Start()
    {
        GameManager.Instance.OnStateChanged += Instance_OnStateChanged;
        Hide();
    }
    private void Awake()
    {
        repawnButton.onClick.AddListener(() =>
        {
            CarController.LocalInstance.Respawn();
        });
    }
    private void Instance_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying())
            Show();
        else Hide();
    }
    private void Update()
    {

        timerText.text = GameManager.Instance.GetGamePlayingTimerNormalized().ToString();
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
