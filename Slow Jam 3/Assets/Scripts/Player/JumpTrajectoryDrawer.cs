using System;
using System.Collections.Generic;
using Systems.Stats;
using UnityEngine;

[ExecuteInEditMode]
public class JumpTrajectoryDrawer : MonoBehaviour
{
	public PlayerMoveStats stats;
	public Vector2 origin;

	public const float HUNDREDTHS_TO_SECONDS = 0.01f;

	private StatFloat _gravity;
	private StatFloat _xVelocity, _yVelocity;
	private StatMultiplier _reverseDirection;
	private StatMultiplier _fallingGravity;
	private StatClamp _maxFallSpeed;

	private RaycastHit2D _raycastHit;

	private Vector2 Velocity => new(_xVelocity.ModifiedValue, _yVelocity.ModifiedValue);

	public void Awake()
	{
		// default values are set elsewhere
		_gravity = new StatFloat(baseValue: default, _fallingGravity);
		_xVelocity = new StatFloat(baseValue: default, _reverseDirection);
		_yVelocity = new StatFloat(baseValue: default, _maxFallSpeed);

		_reverseDirection = new StatMultiplier(multiplier: -1, enabled: false);
		_fallingGravity = new StatMultiplier(multiplier: default, enabled: true);
		_maxFallSpeed = new StatClamp(range: default, enabled: true);
	}

	public void OnDrawGizmos()
	{
		if (stats == null || stats.jumpTrajectory.Hide)
			return;
		var settings = stats.jumpTrajectory;

		Gizmos.color = settings.Color;
		
		// Physics variables
		var timeStep = (settings.MaxLength * HUNDREDTHS_TO_SECONDS) / settings.Resolution;
		Vector2 position = (Vector2)transform.position + origin, displacement;

		_xVelocity.BaseValue = stats.MaxSpeed.BaseValue;
		_yVelocity.BaseValue = stats.JumpSpeed.BaseValue;
		stats.CalculateGravity();
		_gravity.BaseValue = stats.Gravity.BaseValue;

		// stat modifiers
		_fallingGravity.Multiplier = stats.FallingGravity.Multiplier;
		_maxFallSpeed.Min = -stats.MaxFallSpeed.BaseValue;
		_maxFallSpeed.Max = +stats.MaxFallSpeed.BaseValue;

		_reverseDirection.Enabled = stats.jumpTrajectory.direction is JumpTrajectoryGizmoSettings.Direction.Left;
		_fallingGravity.Enabled = false;

		for (int i = 1; i < settings.Resolution; i++)
		{
			_fallingGravity.Enabled = _yVelocity.ModifiedValue < 0;

			// Apply gravity and calculate displacement
			_yVelocity.BaseValue -= _gravity.ModifiedValue * timeStep / 2;
			displacement = Velocity * timeStep;
			_yVelocity.BaseValue -= _gravity.ModifiedValue * timeStep / 2;

			_raycastHit = Physics2D.Raycast(position, displacement.normalized, displacement.magnitude, stats.GroundLayer);

			if (_raycastHit.collider)
			{
				displacement = displacement.normalized * _raycastHit.distance;
			}

			Gizmos.DrawLine(position, position + displacement);
			position += displacement;

			if (!_raycastHit.collider)
				continue;
			else
				break;
		}
	}
}
