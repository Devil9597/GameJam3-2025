using UnityEngine;

public class PlayerVerticalMovement : PlayerMotionController
{
	public const string TR_Y_VELOCITY = "y velocity";

	[SerializeField] private bool _jumpRequested, _jumpCancelled;
	[SerializeField] private bool _isFastFalling;
	[SerializeField] private float _yVelocity;

	public void OnEnable()
	{
		if (InputHandler != null)
		{
			InputHandler.Player.FastFall.performed += this.FastFall_performed;
			InputHandler.Player.Jump.performed += this.Jump_performed;
			InputHandler.Player.Jump.canceled += this.Jump_canceled;
		}
	}

	public void OnDisable()
	{
		if (InputHandler != null)
		{
			InputHandler.Player.FastFall.performed -= this.FastFall_performed;
			InputHandler.Player.Jump.performed -= this.Jump_performed;
			InputHandler.Player.Jump.canceled -= this.Jump_canceled;
		}
	}

	public override void Initialize()
	{
		OnEnable();

		_yVelocity = 0f;
		_jumpRequested = false;
		_jumpCancelled = false;
		_isFastFalling = false;
	}


	public override void BeforeMovement()
	{
		Manger.SetValue(TR_Y_VELOCITY, _yVelocity);

		Stats.AerialJumpHeight.Enabled = Manger.GetValue(PlayerGroundDetector.TR_IS_GROUNDED, out bool _);

		Stats.FallingGravity.Enabled = _yVelocity < 0f;

		Stats.FastFallSpeed.Enabled = _isFastFalling;
	}

	public override void ApplyMovement(in float deltaTime)
	{
		Manger.GetValue(PlayerGroundDetector.TR_IS_GROUNDED, out bool isGrounded);
		Manger.GetValue(PlayerGroundDetector.TR_HIT_CEILING, out bool hitCeiling);

		// Handle Jump
		if (_jumpRequested)
		{
			Jump();
			isGrounded = false;
		}
		else if (_jumpCancelled && _yVelocity > 0)
		{
			StopJump();
		}
		else if (_yVelocity <= 0)
		{
			Manger.SetValue(PlayerGroundDetector.TR_DISABLE_SNAPPING, false);
		}

		// Handle Ceiling
		if (_yVelocity > 0 && hitCeiling)
		{
			StopJump(true);
		}

		// Handle Gravity
		Accelerate(deltaTime, Mathf.Abs(Stats.Gravity.ModifiedValue), -Stats.MaxFallSpeed.ModifiedValue);

		// Handle Ground
		if (isGrounded)
		{
			_isFastFalling = false;
			_jumpCancelled = false;
			_yVelocity = 0;
			Manger.SetValue(TR_Y_VELOCITY, _yVelocity);
		}
		// Handle Fastfall
		else if (_isFastFalling || _yVelocity < -Stats.MaxFallSpeed.ModifiedValue)
		{
			_yVelocity = -Stats.MaxFallSpeed.ModifiedValue;
			Manger.SetValue(TR_Y_VELOCITY, _yVelocity);
		}
	}

	private void Accelerate(in float deltaTime, in float acceleration, in float targetVelocity)
	{
		_yVelocity = Mathf.MoveTowards(_yVelocity, targetVelocity, acceleration * deltaTime / 2);
		Manger.SetValue(TR_Y_VELOCITY, _yVelocity);
		_yVelocity = Mathf.MoveTowards(_yVelocity, targetVelocity, acceleration * deltaTime / 2);
	}

	/// <summary>
	/// Makes the player jump.
	/// </summary>
	/// <param name="force">Set this value to true to allow for midair jumps without consuming resources.</param>
	public void Jump(bool force = false)
	{
		_jumpRequested = false;

		bool canJump = false;
		Manger.GetValue(PlayerGroundDetector.TR_IS_GROUNDED, out bool isGrounded);

		if (isGrounded || force)
		{
			canJump = true;
		}
		else if (Stats.MidairJumps.ModifiedValue > 0)
		{
			Stats.MidairJumpsUsed.Amount++;
			canJump = true;
		}

		if (canJump)
		{
			Manger.SetValue(PlayerGroundDetector.TR_DISABLE_SNAPPING, value: true);
			_yVelocity = Stats.JumpSpeed.ModifiedValue;
		}
	}

	/// <summary>
	/// Multiplies the vertical velocity by the <see cref="PlayerMoveStats.JumpCancelFactor"/>
	/// </summary>
	public void StopJump(bool invert = false)
	{
		_jumpCancelled = false;

		_yVelocity *= Stats.JumpCancelFactor;

		if (invert)
		{
			_yVelocity *= -1;
		}
	}

	#region Event Handlers
	private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		RequestJump();
	}

	private void Jump_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		CancelJump();
	}

	public void RequestJump()
	{
		_jumpRequested = true;
		_jumpCancelled = false;
	}

	public void CancelJump()
	{
		_jumpCancelled = true;
		_jumpRequested = false;
	}

	private void FastFall_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		if (obj.ReadValue<float>() < 0 && !_isFastFalling)
		{
			_isFastFalling = true;
		}
		else if (obj.ReadValue<float>() > 0 && _isFastFalling)
		{
			_isFastFalling = false;
		}
	}
	#endregion
}
