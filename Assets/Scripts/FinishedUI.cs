using UnityEngine;

public class FinishedUI : MonoBehaviour
{
    private void Start()
    {
        InGameMusicManager.Instance.OnGameEnded += Instance_OnGameEnded;
        GameManager.Instance.OnStateChanged += Instance_OnStateChanged;
        Hide();
    }

    private void Instance_OnStateChanged(object sender, System.EventArgs e)
    {
       if (GameManager.Instance.IsGameOver())
        {
            Hide();
        }
    }

    private void Instance_OnGameEnded(object sender, System.EventArgs e)
    {
        Show();
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
