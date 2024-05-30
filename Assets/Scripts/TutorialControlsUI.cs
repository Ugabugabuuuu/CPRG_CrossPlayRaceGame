using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialControlsUI : MonoBehaviour
{
    [SerializeField] Button readyButton;
    private void Start()
    {
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
        Show();
    }
    private void GameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e)
    {
        Hide();
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide() { gameObject.SetActive(false); }

}
