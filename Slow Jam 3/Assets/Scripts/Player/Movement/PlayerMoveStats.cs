using System;
using Systems.Stats;
using UnityEngine;
using Utilities.Timers;

[CreateAssetMenu(fileName = "PlayerMoveStats", menuName = "Scriptable Objects/PlayerMoveStats")]
public class PlayerMoveStats : ScriptableObject
{
	[Header("Horizontal Movement")]
	[SerializeField] private float _maxSpeed = 2.0f;
	[SerializeField] private float _aerialMaxSpeed = 3.0f;
	[SerializeField] private float _runSpeedMult = 2.0f;
	[SerializeField] private float _acceleration = 2.0f;
	[SerializeField] private float _aerialAcceleration = 1.0f;
	[SerializeField] private float _drag = 8.0f;
	[SerializeField] private float _aerialDrag = 1.0f;

	[Header("Jumping")]
	[SerializeField] private float _jumpHeight = 5.0f;
	[SerializeField] private float _aerialJumpHeight = 5.0f;
	[SerializeField] private float _jumpSpeed = 0.5f;
	[SerializeField] private int _midairJumps = 0;
	[SerializeField] private float _jumpBufferTime = 0.25f;

	[Header("Gravity")]
	[SerializeField] private float _maxFallSpeed = 7.5f;
	[SerializeField] private float _fallingGravityMult = 2.0f;
	[SerializeField] private float _fastFallMult = 2.5f;

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
	public StatFloat MaxSpeed { get; private set; }
	public StatFloat Acceleration { get; private set; }
	public StatFloat Drag { get; private set; }

	public StatFloat JumpHeight { get; private set; }
	public StatFloat JumpSpeed { get; private set; }
	public StatInt MidairJumps { get; private set; }

	public StatFloat Gravity { get; private set; }

	public StatFloat MaxFallSpeed { get; private set; }

	// Modifiers
	public StatOverride<float> AerialMaxSpeed { get; private set; }
	public StatMultiplier RunSpeed { get; private set; }
	public StatOverride<float> AerialAcceleration { get; private set; }
	public StatOverride<float> AerialDrag { get; private set; }

	public StatOverride<float> AerialJumpHeight { get; private set; }
	public StatDecrease MidairJumpsUsed { get; private set; }

	public StatMultiplier FallingGravity { get; private set; }

	public StatMultiplier FastFallSpeed { get; private set; }

	// Other Values
	public LayerMask GroundLayer { get => _groundLayer; set => _groundLayer = value; }
	public Rect GroundDetectionArea { get => _detectionArea; set => _detectionArea = value; }

	public CountdownTimer JumpBuffer { get; private set; }
	public CountdownTimer CoyoteTimer { get; private set; }
	#endregion

	public void OnEnable()
	{
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