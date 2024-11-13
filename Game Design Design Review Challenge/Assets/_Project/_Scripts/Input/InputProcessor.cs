using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Create InputProcessor", fileName = "InputProcessor", order = 0)]
public class InputProcessor : ScriptableObject, PlayerInputActions.IPlayerActions {
    public event Action<Vector2> OnMoveEvent = delegate { };
    public event Action OnInteractEvent = delegate { };
    public event Action OnJumpEvent = delegate { };


    PlayerInputActions inputActions;

    void OnEnable() {
        inputActions = new();
        inputActions.Player.SetCallbacks(this);
        inputActions.Player.Enable();
    }

    void OnDisable() { Disable(); }

    public void Enable() { inputActions.Player.Enable(); }

    public void Disable() { inputActions.Player.Disable(); }

    public void OnMove(InputAction.CallbackContext context) { OnMoveEvent.Invoke(context.ReadValue<Vector2>()); }

    public void OnInteract(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            OnInteractEvent.Invoke();
        }
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            OnJumpEvent.Invoke();
        }
    }
}