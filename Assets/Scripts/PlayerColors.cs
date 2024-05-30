using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColors : MonoBehaviour
{
    [SerializeField] private MeshRenderer carBodyMeshRenderer;
    [SerializeField] private MeshRenderer carChassisMeshRenderer;

    private Material material;
    private void Awake()
    {
        material = new Material(carBodyMeshRenderer.material);
        carBodyMeshRenderer.material = material;
        carChassisMeshRenderer.material = material;
    }
    public void SetPlayerCarColor(Color color)
    {
        material.color = color;
    }
}
