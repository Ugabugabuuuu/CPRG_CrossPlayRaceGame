using System;
using TMPro;
using UnityEngine;

public class StartCountDownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI startCountdownText;
    private int previousCountdown = 4;
    private void Start()
    { 
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        { 
            Show();
        }
        else 
        { 
            Hide();
        }
    }
    private void Update()
    {


        startCountdownText.text = Mathf.CeilToInt(Convert.ToInt32(GameManager.Instance.GetCountdownToStartTimer())).ToString();
        if(Mathf.CeilToInt(Convert.ToInt32(GameManager.Instance.GetCountdownToStartTimer()))==0)
        {
           startCountdownText.text = "GO!";
        }
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
