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
		if (_input.Multiplier != 0f && Mathf.Abs(_xVelocity) < Stats.MaxSpeed.ModifiedValue)
		{
			// Use acceleration
			Accelerate(deltaTime, Stats.Acceleration.ModifiedValue, Stats.MaxSpeed.ModifiedValue);
		}
		else if (Mathf.Abs(_xVelocity) >= Stats.MaxSpeed.ModifiedValue)
		{
			// Use drag to decelerate to a lower max speed
			Accelerate(deltaTime, Stats.Drag.ModifiedValue, Stats.MaxSpeed.ModifiedValue);
		}
		else
		{
			// use drag
			Accelerate(deltaTime, Stats.Drag.ModifiedValue, 0);
		}
	}

	private void Accelerate(in float deltaTime, in float acceleration, in float targetVelocity)
	{
		// Acceleration is split in half for proper interpolation.
		_xVelocity = Mathf.MoveTowards(_xVelocity, targetVelocity, acceleration * deltaTime / 2);
		Manger.SetValue(TR_X_VELOCITY, _xVelocity);
		_xVelocity = Mathf.MoveTowards(_xVelocity, targetVelocity, acceleration * deltaTime / 2);
	}
}
