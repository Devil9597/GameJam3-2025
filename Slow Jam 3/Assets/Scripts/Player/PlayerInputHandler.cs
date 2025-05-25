using System;
using UnityEngine;
using UnityEngine.InputSystem;

using static InputSystem_Actions;
using static UnityEngine.InputSystem.InputAction;

[CreateAssetMenu(fileName = "NewInputHandler", menuName = "Scriptable Objects/Player Input Handler")]
public class PlayerInputHandler : ScriptableObject
{
	private InputSystem_Actions _actions;

	public PlayerActions Player => _actions.Player;
	public UIActions UI => _actions.UI;

	public void OnEnable()
	{
		_actions = new();
		_actions.Enable();
	}

	public void OnDisable()
	{
		_actions.Disable();
		_actions = null;
	}
}

public static class InputHelpers
{
	[Flags]
	public enum ContextPhase
	{
		All = -1, None = 0, Started = 1, Performed = 2, Cancelled = 4,
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
