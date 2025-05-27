using System;
using UnityEngine;
using Utilities.Extensions;
using Utilities.Serializables;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMotionManger : MonoBehaviour
{
	private Rigidbody2D _body;

	public bool isGrounded;
	public PlayerInputHandler inputHandler;
	public PlayerMoveStats stats;
	[Space]
	[Tooltip("The controllers managed by this script. Each controller is processed in order of the array.")]
	public PlayerMotionController[] controllers;
	[Tooltip("These values persist for a single frame, where all controllers may access them at any time.")]
	[SerializeField] private Dictionary<string, float> _transients = new();

	public void Start()
	{
		TryGetComponent(out _body);

		Action initialize = delegate { };
		for (int i = 0; i < controllers.Length; i++)
		{
			controllers[i].Manger = this;
			controllers[i].Body = _body;
			controllers[i].InputHandler = inputHandler;
			controllers[i].Stats = stats;

			initialize += controllers[i].Initialize;
		}
		initialize();
	}

	public void FixedUpdate()
	{
		void iterateControllers(System.Action<PlayerMotionController> action)
		{
			for (int i = 0; i < controllers.Length; i++)
			{
				if (controllers[i] != null && controllers[i].enabled)
				{
					action(controllers[i]);
				}
			}
		}

		iterateControllers(c => c.BeforeMovement());
		iterateControllers(c => c.ApplyMovement(Time.fixedDeltaTime));
		iterateControllers(c => c.AfterMovement());
	}

	[ContextMenu("Get Controllers")]
	private void GetControllers()
	{
		controllers = this.Get().Components<PlayerMotionController>().InChildren();
	}

	#region Transient Accessors
	public void SetTransient(string name, bool value) => SetTransient(name, value ? 1 : 0);

	public void SetTransient(string name, bool? value) => SetTransient(name, value switch { true => 1, null => 0, false => -1 });

	public void SetTransient(string name, int value) => SetTransient(name, (float)value);

	public void SetTransient(string name, float value)
	{
		if (_transients.ContainsKey(name))
		{
			_transients[name] = value;
		}
		else
		{
			_transients.Add(name, value);
		}
	}

	public void GetTransient(string name, out bool value)
	{
		if (!_transients.ContainsKey(name))
		{
			value = default;
			return;
		}

		GetTransient(name, out float result);
		value = result switch {
			> 0 => true,
			< 0 => false,
			_ => false
		};
	}

	public void GetTransient(string name, out bool? value)
	{
		if (!_transients.ContainsKey(name))
		{
			value = default;
			return;
		}

		GetTransient(name, out float result);
		value = result switch {
			> 0 => true,
			0 => null,
			< 0 => false,
			float.NaN => throw new NotFiniteNumberException(result)
		};
	}

	public void GetTransient(string name, out int value)
	{
		if (!_transients.ContainsKey(name))
		{
			value = default;
			return;
		}

		GetTransient(name, out float result);
		value = (int)result;
	}

	public void GetTransient(string name, out float value)
	{
		if (_transients.ContainsKey(name))
		{
			value = _transients[name];
		}
		else
		{
			value = default;
		}
	}
	#endregion
}
