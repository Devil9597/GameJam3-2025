using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem_Actions;
using static UnityEngine.InputSystem.InputAction;

[CreateAssetMenu(fileName = "NewInputHandler", menuName = "Scriptable Objects/Player Input Handler")]
public class PlayerInputHandler : ScriptableObject
{
    public InputSystem_Actions Actions { get; private set; }

    public PlayerActions Player => Actions.Player;
    public UIActions UI => Actions.UI;
    public ClimbActions ClimbActions => Actions.Climb;

    public void OnEnable()
    {
        Actions = new();
        Actions.Enable();
    }

    public void OnDisable()
    {
        Actions.Disable();
        Actions = null;
    }
}

public static class InputHelpers
{
    [Flags]
    public enum ContextPhase
    {
        All = -1,
        None = 0,
        Started = 1,
        Performed = 2,
        Cancelled = 4,
    }

    public static void AddControl(this InputAction action, ContextPhase phase, Action<CallbackContext> callback)
    {
        if (phase is ContextPhase.None) return;
        if (phase is ContextPhase.All)
        {
            action.started += callback;
            action.performed += callback;
            action.canceled += callback;
        }
        else
        {
            if (phase.HasFlag(ContextPhase.Started))
                action.started += callback;
            if (phase.HasFlag(ContextPhase.Performed))
                action.performed += callback;
            if (phase.HasFlag(ContextPhase.Cancelled))
                action.canceled += callback;
        }
    }

    public static void RemoveControl(this InputAction action, ContextPhase phase, Action<CallbackContext> callback)
    {
        if (phase is ContextPhase.None) return;
        if (phase is ContextPhase.All)
        {
            action.started -= callback;
            action.performed -= callback;
            action.canceled -= callback;
        }
        else
        {
            if (phase.HasFlag(ContextPhase.Started))
                action.started -= callback;
            if (phase.HasFlag(ContextPhase.Performed))
                action.performed -= callback;
            if (phase.HasFlag(ContextPhase.Cancelled))
                action.canceled -= callback;
        }
    }
}