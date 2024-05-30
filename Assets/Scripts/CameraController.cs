using UnityEngine;
using Unity.Netcode;

public class CameraController : NetworkBehaviour
{
    private Transform carTransform;
    public Vector3 offset = new Vector3(0f, 3f, -6f);

    void Start()
    {
        if (!IsOwner)
            return;
        if (transform.parent != null)
        {
            carTransform = transform.parent;
        }
    }

    void LateUpdate()
    {
        if (carTransform != null)
        {
            Vector3 desiredPosition = carTransform.position + carTransform.TransformDirection(offset);
            transform.position = desiredPosition;
            transform.LookAt(carTransform);
        }
    }
}
