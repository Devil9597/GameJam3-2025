using System;
using Systems.Stats;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMoveStats", menuName = "Scriptable Objects/PlayerMoveStats")]
public class PlayerMoveStats : ScriptableObject
{
	[Header("Horizontal Movement")]
	[SerializeField] private StatFloat _maxSpeed = 2.0f;
	[SerializeField] private float _groundedAcceleration = 2.0f;
	[SerializeField] private float _groundedDrag = 8.0f;
	[SerializeField] private float _aerialAcceleration = 1.0f;
	[SerializeField] private float _aerialDrag = 1.0f;
	[SerializeField] private float _runSpeedMult = 2.0f;

	[Header("Jumping")]
	[SerializeField] private StatFloat _jumpHeight = 5.0f;
	[SerializeField] private float _aerialJumpHeight = 5.0f;
	[SerializeField] private StatFloat _jumpSpeed = 0.5f;
	[SerializeField] private StatInt _midairJumps = 0;
	[SerializeField] private float _jumpBufferTime = 0.25f;

	[Header("Gravity")]
	[SerializeField] private StatFloat _maxFallSpeed = 7.5f;
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

	public StatFloat JumpHeight { get; private set; }
	public StatFloat JumpSpeed { get; private set; }
	public StatInt MidairJumps { get; private set; }

	public StatFloat Gravity { get; private set; }

	public StatFloat MaxFallSpeed { get; private set; }

	// Modifiers
	//public StatMultiplier GroundedAcceleration { get; private set; }
	//public StatMultiplier 
	#endregion
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