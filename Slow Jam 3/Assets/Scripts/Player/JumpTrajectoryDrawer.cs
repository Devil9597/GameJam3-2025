using System;
using System.Collections.Generic;
using Systems.Stats;
using UnityEngine;

public class JumpTrajectoryDrawer : MonoBehaviour
{
	public PlayerMoveStats stats;
	public Vector2 origin;

	private List<Vector2> _points = new();

	public const float HUNDREDTHS_TO_SECONDS = 0.01f;

	private StatFloat _xVelocity, _yVelocity;
	private StatFloat _maxXVelocity, _maxYVelocity;

	private StatClamp _xClamp, _yClamp;

	public void OnDrawGizmos()
	{
		if (stats == null || stats.jumpTrajectory.Hide)
			return;
		var settings = stats.jumpTrajectory;
		
		_points.Clear();
		_points.Capacity = settings.MaxLength;
		
		var timeStep = (settings.MaxLength * HUNDREDTHS_TO_SECONDS) / settings.Resolution;

		// V = (ground speed, jump speed)
		
		// D = (origin + transform position)
		Vector2 position = (Vector2)transform.position + origin;

		_points.Add(position);


		/*
		 * Each step:
		 * apply gravity up to max fall speed. if descending, apply the falling modifier
		 * apply drag up to max air speed
		 * move the position by the velocity
		 * check for collisions, and break out if any.
		 * otherwise, repeat.
		 */
	}
}
