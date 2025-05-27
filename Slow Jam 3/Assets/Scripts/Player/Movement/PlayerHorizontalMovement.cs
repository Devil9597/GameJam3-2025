using Systems.Stats;
using UnityEngine;
using Utilities.Serializables;

public class PlayerHorizontalMovement : PlayerMotionController
{
	public const string TR_X_VELOCITY = "x velocity";

	[SerializeField] private StatMultiplier _input;
	[SerializeField] private float _xVelocity;

	public void OnEnable()
	{
		if (InputHandler != null)
		{
			InputHandler.Player.Move.AddControl(InputHelpers.ContextPhase.All, Move_performed);
			InputHandler.Player.Sprint.performed += Sprint_performed;
			InputHandler.Player.Sprint.canceled += Sprint_canceled;
		}
	}

	public void OnDisable()
	{
		if (InputHandler != null)
		{
			InputHandler.Player.Move.RemoveControl(InputHelpers.ContextPhase.All, Move_performed);
			InputHandler.Player.Sprint.performed -= Sprint_performed;
			InputHandler.Player.Sprint.canceled -= Sprint_canceled;
		}
	}

	public override void Initialize()
	{
		OnEnable();

		_input = new(0f);
		_xVelocity = 0f;
		Stats.RunSpeed.Enabled = false;

		Stats.Acceleration.AddModifier(_input);
	}

	private void Sprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		Stats.RunSpeed.Enabled = true;
	}

	private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		Stats.RunSpeed.Enabled = false;
	}

	private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		_input.Multiplier = obj.ReadValue<float>();
	}

	public override void BeforeMovement()
	{
		Manger.SetValue(TR_X_VELOCITY, _xVelocity);

		Manger.GetValue(PlayerGroundDetector.TR_IS_GROUNDED, out bool isGrounded);

		Stats.AerialMaxSpeed.Enabled = !isGrounded;
		Stats.AerialAcceleration.Enabled = !isGrounded;
		Stats.AerialDrag.Enabled = !isGrounded;
	}

	public override void ApplyMovement(in float deltaTime)
	{
		Debug.Assert(Stats.MaxSpeed.ModifiedValue >= 0);
		if (_input.Multiplier != 0 && Mathf.Abs(_xVelocity) <= Stats.MaxSpeed.ModifiedValue)
		{
			// Use acceleration
			Debug.Log("Acceleration: " + Stats.Acceleration.ModifiedValue);
			Accelerate(deltaTime, Stats.Acceleration.ModifiedValue, Stats.MaxSpeed.ModifiedValue * Mathf.Sign(Stats.Acceleration.ModifiedValue));
		}
		else
		{
			// use drag
			Debug.Log("Drag");
			Accelerate(deltaTime, Stats.Drag.ModifiedValue, 0);
		}
	}

	private void Accelerate(in float deltaTime, float acceleration, float targetVelocity)
	{
		Debug.Log(targetVelocity);
		acceleration = Mathf.Abs(acceleration);
		// Acceleration is split in half for proper interpolation.
		_xVelocity = Mathf.MoveTowards(_xVelocity, targetVelocity, acceleration * deltaTime / 2);
		Manger.SetValue(TR_X_VELOCITY, _xVelocity);
		_xVelocity = Mathf.MoveTowards(_xVelocity, targetVelocity, acceleration * deltaTime / 2);
	}
}
