using System;
using Unity.Netcode;
using UnityEngine;

public class PlatformUIController : NetworkBehaviour
{
    public Canvas mobileControlsCanvas;
    public static PlatformUIController Instance { get; private set; }
    private void Awake()
    {
        Hide();
        Instance = this;

    }
    void Start()
    {
  
        Hide();

        if (IsOwner)
        {

                Hide();
            if (Application.platform == RuntimePlatform.Android)
            {
                GameManager.Instance.OnStateChanged += Instance_OnStateChanged;
                GameManager.Instance.OnFinished += PlatformUIController_OnFinished;
            }
            else
            {
                mobileControlsCanvas.enabled = false;
            }
        }
    }

    private void PlatformUIController_OnFinished(object sender, EventArgs e)
    {
            Hide();
    }

    private void Instance_OnStateChanged(object sender, System.EventArgs e)
    {

        if (GameManager.Instance.IsCountdownToStartActive())
        {
            Show();
        }

    }
    private void Show()
    {
        mobileControlsCanvas.enabled = true;
    }
    private void Hide()
    {
        mobileControlsCanvas.enabled = false;
    }

}
