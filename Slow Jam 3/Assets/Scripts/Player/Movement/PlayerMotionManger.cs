using System;
using UnityEngine;
using Utilities.Extensions;
using Utilities.Serializables;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMotionManger : MonoBehaviour
{
	private Rigidbody2D _body;

	public PlayerInputHandler inputHandler;
	public PlayerMoveStats stats;
	[Space]
	[Tooltip("The controllers managed by this script. Each controller is processed in order of the array.")]
	public PlayerMotionController[] controllers;
	[Tooltip("These values persist for a single frame, where all controllers may access them at any time.")]
	[SerializeField] private Dictionary<string, float> _dynamicValues = new();

	private Action _clearTransients;

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
		_clearTransients?.Invoke();

		iterateControllers(c => c.BeforeMovement());
		iterateControllers(c => c.ApplyMovement(Time.fixedDeltaTime));
		iterateControllers(c => c.AfterMovement());

		void iterateControllers(Action<PlayerMotionController> action)
		{
			for (int i = 0; i < controllers.Length; i++)
			{
				if (controllers[i] != null && controllers[i].enabled)
				{
					action(controllers[i]);
				}
			}
		}
	}

	[ContextMenu("Get Controllers")]
	private void GetControllers()
	{
		controllers = this.Get().Components<PlayerMotionController>().InChildren();
	}

	#region Dynamic Value Accessors
	public void SetValue(string name, bool value, bool transient = false) => this.SetValue(name, (float)(value ? 1 : 0), transient);

	public void SetValue(string name, bool? value, bool transient = false) => this.SetValue(name, (float)(value switch { true => 1, null => 0, false => -1 }), transient);

	public void SetValue(string name, int value, bool transient = false) => SetValue(name, (float)value, transient);

	public void SetValue(string name, float value, bool transient = false)
	{
		if (_dynamicValues.ContainsKey(name))
		{
			_dynamicValues[name] = value;
		}
		else
		{
			_dynamicValues.Add(name, value);
			
			if (transient)
				_clearTransients += clearTransient;

			void clearTransient()
			{
				_dynamicValues.Remove(name);
				_clearTransients -= clearTransient;
			}
		}
	}

	public bool GetValue(string name, out bool value)
	{
		if (!_dynamicValues.ContainsKey(name))
		{
			value = default;
			return value;
		}

		GetValue(name, out float result);
		value = result switch {
			> 0 => true,
			< 0 => false,
			_ => false
		};
		return value;
	}

	public bool? GetValue(string name, out bool? value)
	{
		if (!_dynamicValues.ContainsKey(name))
		{
			value = default;
			return value;
		}

		GetValue(name, out float result);
		value = result switch {
			> 0 => true,
			0 => null,
			< 0 => false,
			float.NaN => throw new NotFiniteNumberException(result)
		};
		return value;
	}

	public int GetValue(string name, out int value)
	{
		if (!_dynamicValues.ContainsKey(name))
		{
			value = default;
			return value;
		}

		GetValue(name, out float result);
		value = (int)result;
		return value;
	}

	public float GetValue(string name, out float value)
	{
		if (_dynamicValues.ContainsKey(name))
		{
			value = _dynamicValues[name];
		}
		else
		{
			value = default;
		}
		return value;
	}
	#endregion
}
