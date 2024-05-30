using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDataSingleUI : MonoBehaviour
{
    [SerializeField] private Button buttonIteraction;
    private AudioSource buttonClickAudioSource;
    private GameObject statusUI;
    Transform exitButtonTransform;
    private void Awake()
    {
        statusUI = GameObject.Find("StatusUI");
        exitButtonTransform = statusUI.transform.Find("ExitButton");
        GameObject buttonClickObject = GameObject.FindWithTag("buttonClickSound");

        if (buttonClickObject != null)
        {

            buttonClickAudioSource = buttonClickObject.GetComponent<AudioSource>();
        }
        buttonIteraction.onClick.AddListener(()=>
        {
                        exitButtonTransform.gameObject.SetActive(false);
            buttonClickAudioSource.Play();
        });
    }
}
