using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkCleanUp : MonoBehaviour
{
    private void Awake()
    {
#if !UNITY_INCLUDE_TESTS
        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);
        if(MultiplayerManager.Instance.gameObject!= null)
            Destroy(MultiplayerManager.Instance.gameObject);
        if(CarGameLobby.Instance != null)
            Destroy (CarGameLobby.Instance.gameObject);
#endif
    }

}
