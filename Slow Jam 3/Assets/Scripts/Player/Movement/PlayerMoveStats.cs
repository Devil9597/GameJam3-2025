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
	[SerializeField, Range(-1.0f, +1.0f)] private float _jumpCancelFactor = 0.1f;

	[Header("Gravity")]
	[SerializeField] private StatFloat _maxFallSpeed = 7.5f;
	[SerializeField] private StatMultiplier _fallingGravityMult = new(2.0f);
	[SerializeField] private StatMultiplier _fastFallMult = new(2.5f);

	[Header("Ground Detection")]
	[SerializeField] private LayerMask _groundLayer = Physics2D.DefaultRaycastLayers;
	[SerializeField] private Rect _floorDetectionArea = new(-0.25f, -1, 0.5f, 0.2f);
	[SerializeField] private float _snappingRayLength = 1f;
	[SerializeField] private Rect _ceilingDetectionArea = new(-0.25f, 1, 0.5f, 0.2f);
	[SerializeField] private float _coyoteTime = 0.25f;


#if UNITY_EDITOR
	[Header("Debug Settings")]
	public GroundCastGizmoSettings floorCast = new() {
		Color = Color.yellow
	};
	public GroundCastGizmoSettings roofCast = new() {
		Color = Color.greenYellow,
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
	public float JumpCancelFactor { get => _jumpCancelFactor; set => _jumpCancelFactor = value; }

	public LayerMask GroundLayer { get => _groundLayer; set => _groundLayer = value; }
	public Rect FloorDetectionArea { get => _floorDetectionArea; set => _floorDetectionArea = value; }
	public float SnappingRayLength { get => _snappingRayLength; set => _snappingRayLength = value; }
	public Rect CeilingDetectionArea { get => _ceilingDetectionArea; set => _ceilingDetectionArea = value; }

	public CountdownTimer CoyoteTimer { get; private set; }
	#endregion

	public void OnEnable()
	{
		Gravity = new();

		CoyoteTimer = new CountdownTimer(_coyoteTime);

		ResetModifiers();
		CalculateGravity();
	}

	public void OnValidate()
	{
#if UNITY_EDITOR
		UnityEditor.Undo.RecordObject(this, name);

		_maxSpeed.SetDirty();
		_acceleration.SetDirty();
		_drag.SetDirty();

		_jumpHeight.SetDirty();
		_jumpSpeed.SetDirty();
		_midairJumps.SetDirty();

		_maxFallSpeed.SetDirty();
		CalculateGravity();
#endif
	}

	public void CalculateGravity()
	{
		float t = JumpHeight.ModifiedValue / JumpSpeed.ModifiedValue;
		Gravity ??= new();
		Gravity.BaseValue = -(JumpHeight.ModifiedValue / Mathf.Pow(t, 2));
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
		AerialJumpHeight.OnValueChange -= CalculateGravity;

		// Re-add persistent modifiers and callbacks
		MaxSpeed.AddModifiers(AerialMaxSpeed, RunSpeed);
		Acceleration.AddModifier(AerialAcceleration);
		Drag.AddModifier(AerialDrag);

		AerialJumpHeight.OnValueChange += CalculateGravity;
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
	public enum Direction { Right, Left };

	public bool Hide;
	public Color Color;

	public Direction direction;
	public int Resolution;
	public int MaxLength;

	public readonly bool Visible => !Hide;
}