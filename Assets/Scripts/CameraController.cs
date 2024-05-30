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
        else
        {
          //  Debug.LogError("CameraController: Camera must be a child object of the car.");
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
