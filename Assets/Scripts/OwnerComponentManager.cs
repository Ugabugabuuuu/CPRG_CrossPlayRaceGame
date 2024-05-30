using UnityEngine;
using Unity.Netcode;

public class OwnerComponentManager : NetworkBehaviour
{
    [SerializeField] private Camera _camera;

    private AudioListener _playerAudioListener;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        AudioListener[] allAudioListeners = FindObjectsOfType<AudioListener>();
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (AudioListener audioListener in allAudioListeners)
        {
            NetworkObject networkObject = audioListener.GetComponentInParent<NetworkObject>();
            if (networkObject == null || networkObject.OwnerClientId != NetworkManager.Singleton.LocalClientId)
            {
                audioListener.enabled = false;
            }
        }
        foreach (Camera camera in cameras)
        {
            NetworkObject networkObject = camera.GetComponentInParent<NetworkObject>();
            if (networkObject == null || networkObject.OwnerClientId != NetworkManager.Singleton.LocalClientId)
            {
                camera.enabled = false;
            }
        }
    }
}
