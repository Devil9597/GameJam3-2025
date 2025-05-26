using Systems.Stats;
using UnityEngine;
using Utilities.Serializables;

public class PlayerHorizontalMovement : PlayerMotionController
{
	private StatMultiplier _input;
	private float _xVelocity;

	public override void Initialize()
	{
		InputHandler.Player.Move.AddControl(InputHelpers.ContextPhase.All, Move_performed);
		InputHandler.Player.Sprint.performed += this.Sprint_performed;
		InputHandler.Player.Sprint.canceled += this.Sprint_canceled;

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
		Stats.AerialMaxSpeed.Enabled = !Manger.isGrounded;
		Stats.AerialAcceleration.Enabled = !Manger.isGrounded;
		Stats.AerialDrag.Enabled = !Manger.isGrounded;
	}

	public override void ApplyMovement(in float deltaTime)
	{
		if (_input.Multiplier != 0f)
		{
			// Use acceleration
			Accelerate(deltaTime, Stats.Acceleration.ModifiedValue, Stats.MaxSpeed.ModifiedValue);
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
		Body.linearVelocityX = _xVelocity;
		_xVelocity = Mathf.MoveTowards(_xVelocity, targetVelocity, acceleration * deltaTime / 2);
	}
}
