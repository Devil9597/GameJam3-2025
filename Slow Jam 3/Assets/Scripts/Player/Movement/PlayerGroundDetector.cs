using UnityEngine;
using Utilities.Serializables;

public class PlayerGroundDetector : PlayerMotionController
{
	public const string TR_IS_GROUNDED = "Is Grounded", TR_HIT_CEILING = "Hit Ceiling";

	[SerializeField] private Vector2 _castOrigin;
	[SerializeField] private TimeSlice _castingPeriod;

	private RaycastHit2D _groundHit;

	public override void Initialize()
	{
		// nop
	}

	public override void BeforeMovement()
	{
		if (_castingPeriod.Update())
		{
			GetFloorRect(Stats, out var center, out var size);
			_groundHit = Physics2D.BoxCast(center, size, angle: 0, direction: Vector2.zero, distance: 0, layerMask: Stats.GroundLayer);

			Manger.SetValue(TR_IS_GROUNDED, _groundHit.collider != null);

			GetCeilingRect(Stats, out center, out size);
			_groundHit = Physics2D.BoxCast(center, size, angle: 0, direction: Vector2.zero, distance: 0, layerMask: Stats.GroundLayer);
			Manger.SetValue(TR_HIT_CEILING, _groundHit.collider != null);
		}
	}

	public override void ApplyMovement(in float deltaTime)
	{
		// nop
	}

	public void OnDrawGizmos()
	{
		// The stats always null while inside the editor
		var stats = Stats;
		if (stats == null)
			stats = GetComponentInParent<PlayerMotionManger>().stats;

		if (stats == null)
		{
			Debug.LogWarning("Ground cast cannot be shown if player stats are not assigned.");
			return;
		}

		if (stats.groundCast.Visible)
		{
			Gizmos.color = stats.groundCast.Color;
			
			GetFloorRect(stats, out var center, out var size);
			Gizmos.DrawCube(center, size);

			GetCeilingRect(stats, out center, out size);
			Gizmos.DrawCube(center, size);
		}
	}

	private void GetFloorRect(in PlayerMoveStats stats, out Vector2 center, out Vector2 size)
	{
		center = ((Vector2)transform.position) + _castOrigin + stats.FloorDetectionArea.center;
		size = Vector2.Scale(stats.FloorDetectionArea.size, (Vector2)transform.localScale);
	}

	private void GetCeilingRect(in PlayerMoveStats stats, out Vector2 center, out Vector2 size)
	{
		center = (Vector2)transform.position + _castOrigin + stats.CeilingDetectionArea.center;
		size = Vector2.Scale(stats.FloorDetectionArea.size, (Vector2)transform.localScale);
	}
}
