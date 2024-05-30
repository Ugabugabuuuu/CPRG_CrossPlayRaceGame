using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarColorSelectUIElement : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image selectedImage;
    [SerializeField] private GameObject selectedGameObject;

    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerManager_OnPlayerDataNetworkListChanged;
        selectedImage.color = MultiplayerManager.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();

    }
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.ChangePlayerCarColor(colorId);
        });
    }
    private void MultiplayerManager_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }
    private void UpdateIsSelected()
    {
        if(MultiplayerManager.Instance.GetPlayerData().colorId == colorId)
        {
            selectedGameObject.SetActive(true);
        }
        else
        {
           // Debug.Log(MultiplayerManager.Instance.GetPlayerData().colorId + "did not matched  " + colorId);
            selectedGameObject.SetActive(false);
        }
    }
}
