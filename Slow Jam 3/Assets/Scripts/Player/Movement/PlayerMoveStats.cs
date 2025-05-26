using System;
using Systems.Stats;
using UnityEngine;
using Utilities.Timers;

[CreateAssetMenu(fileName = "PlayerMoveStats", menuName = "Scriptable Objects/PlayerMoveStats")]
public class PlayerMoveStats : ScriptableObject
{
	[Header("Horizontal Movement")]
	[SerializeField] private StatFloat _maxSpeed = 2.0f;
	[SerializeField] private StatOverride<float> _aerialMaxSpeed = new(3.0f);
	[SerializeField] private StatMultiplier _runSpeedMult = new(2.0f);
	[SerializeField] private StatFloat _acceleration = 2.0f;
	[SerializeField] private StatOverride<float> _aerialAcceleration = new(1.0f);
	[SerializeField] private StatFloat _drag = 8.0f;
	[SerializeField] private StatOverride<float> _aerialDrag = new(1.0f);

	[Header("Jumping")]
	[SerializeField] private StatFloat _jumpHeight = 5.0f;
	[SerializeField] private StatOverride<float> _aerialJumpHeight = new(5.0f);
	[SerializeField] private StatFloat _jumpSpeed = 0.5f;
	[SerializeField] private StatInt _midairJumps = 0;
	[SerializeField] private float _jumpBufferTime = 0.25f;

	[Header("Gravity")]
	[SerializeField] private StatFloat _maxFallSpeed = 7.5f;
	[SerializeField] private StatMultiplier _fallingGravityMult = new(2.0f);
	[SerializeField] private StatMultiplier _fastFallMult = new(2.5f);

	[Header("Ground Detection")]
	public LayerMask _groundLayer = Physics2D.DefaultRaycastLayers;
	public Rect _detectionArea = new(0, -1, 0.5f, 0.2f);
	[SerializeField] private float _coyoteTime = 0.25f;

#if UNITY_EDITOR
	[Header("Debug Settings")]
	public GroundCastGizmoSettings groundCast = new() {
		Color = Color.yellow
	};
	public JumpTrajectoryGizmoSettings jumpTrajectory = new() {
		Color = Color.white,
		MaxLength = 100,
		Resolution = 20
	};
#endif

	#region Public Properties
	// Stat values
	public StatFloat MaxSpeed { get => _maxSpeed; private set => _maxSpeed = value; }
	public StatFloat Acceleration { get => _acceleration; private set => _acceleration = value; }
	public StatFloat Drag { get => _drag; private set => _drag = value; }

	public StatFloat JumpHeight { get => _jumpHeight; private set => _jumpHeight = value; }
	public StatFloat JumpSpeed { get => _jumpSpeed; private set => _jumpSpeed = value; }
	public StatInt MidairJumps { get => _midairJumps; private set => _midairJumps = value; }

	public StatFloat Gravity { get; private set; }

	public StatFloat MaxFallSpeed { get => _maxFallSpeed; private set => _maxFallSpeed = value; }

	// Modifiers
	public StatOverride<float> AerialMaxSpeed { get => _aerialMaxSpeed; private set => _aerialMaxSpeed = value; }
	public StatMultiplier RunSpeed { get => _runSpeedMult; private set => _runSpeedMult = value; }
	public StatOverride<float> AerialAcceleration { get => _aerialAcceleration; private set => _aerialAcceleration = value; }
	public StatOverride<float> AerialDrag { get => _aerialDrag; private set => _aerialDrag = value; }

	public StatOverride<float> AerialJumpHeight { get => _aerialJumpHeight; private set => _aerialJumpHeight = value; }
	public StatDecrease MidairJumpsUsed { get; private set; }

	public StatMultiplier FallingGravity { get => _fallingGravityMult; private set => _fallingGravityMult = value; }

	public StatMultiplier FastFallSpeed { get => _fastFallMult; private set => _fastFallMult = value; }

	// Other Values
	public LayerMask GroundLayer { get => _groundLayer; set => _groundLayer = value; }
	public Rect GroundDetectionArea { get => _detectionArea; set => _detectionArea = value; }

	public CountdownTimer JumpBuffer { get; private set; }
	public CountdownTimer CoyoteTimer { get; private set; }
	#endregion

	public void OnEnable()
	{
		Gravity = new();

		JumpBuffer = new CountdownTimer(_jumpBufferTime);
		CoyoteTimer = new CountdownTimer(_coyoteTime);

		ResetModifiers();
		CalculateValues();
	}

	private void CalculateValues()
	{
		float t = JumpHeight.ModifiedValue / JumpSpeed.ModifiedValue;
		Gravity.BaseValue = -(2f * JumpHeight.ModifiedValue) / t;
	}

	private void ResetModifiers()
	{
		// Remove all modifiers from all stats
		MaxSpeed.ClearModifiers();
		Acceleration.ClearModifiers();
		Drag.ClearModifiers();

		JumpHeight.ClearModifiers();
		JumpSpeed.ClearModifiers();
		MidairJumps.ClearModifiers();

		Gravity.ClearModifiers();
		MaxFallSpeed.ClearModifiers();

		// Remove additional callbacks
		AerialJumpHeight.OnValueChange -= CalculateValues;

		// Re-add persistent modifiers and callbacks
		MaxSpeed.AddModifiers(AerialMaxSpeed, RunSpeed);
		Acceleration.AddModifier(AerialAcceleration);
		Drag.AddModifier(AerialDrag);

		AerialJumpHeight.OnValueChange += CalculateValues;
		JumpHeight.AddModifier(AerialJumpHeight);
		//JumpSpeed
		//MidairJumps

		Gravity.AddModifier(FallingGravity);
		MaxFallSpeed.AddModifier(FastFallSpeed);
	}
}

[Serializable]
public struct GroundCastGizmoSettings
{
	public bool Hide;
	public Color Color;

	public readonly bool Visible => !Hide;
}

[Serializable]
public struct JumpTrajectoryGizmoSettings
{
	public bool Hide;
	public Color Color;

	public int Resolution;
	public int MaxLength;

	public readonly bool Visible => !Hide;
}