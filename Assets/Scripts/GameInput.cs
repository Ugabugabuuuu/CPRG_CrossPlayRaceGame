using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {

    public static GameInput Instance { get; private set; }

    public event EventHandler OnInteractAction;

    public event EventHandler onRespawn;

    private PlayerControls playerInputActions;


    private void Awake() {
        Instance = this;


        playerInputActions = new PlayerControls();

        playerInputActions.CarControls.Enable();

        playerInputActions.CarControls.Iteraction.performed += Interact_performed;
        playerInputActions.CarControls.Respawn.performed += Interact_onRespawn;
    }

    private void OnDestroy() {
        playerInputActions.CarControls.Iteraction.performed -= Interact_performed;
 
        playerInputActions.CarControls.Respawn.performed -= Interact_onRespawn;

        playerInputActions.Dispose();
    }
    private void Interact_onRespawn(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        onRespawn?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    } 
}