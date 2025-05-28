using UnityEngine;
using Utilities.Extensions;
using Utilities.Serializables;

public class PlayerGroundDetector : PlayerMotionController
{
	public const string
		TR_IS_GROUNDED = "Is Grounded",
		TR_HIT_CEILING = "Hit Ceiling",
		TR_DISABLE_SNAPPING = "Snapping Disabled";

	[Tooltip("The y position of the player's feet, relative to rigidbody position.")]
	[SerializeField] private float _feetPosition;
	[SerializeField] private TimeSlice _castingPeriod;

	private RaycastHit2D _groundHit, _roofHit;

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
			_roofHit = Physics2D.BoxCast(center, size, angle: 0, direction: Vector2.zero, distance: 0, layerMask: Stats.GroundLayer);
			Manger.SetValue(TR_HIT_CEILING, _roofHit.collider != null);
		}
	}

	public override void ApplyMovement(in float deltaTime)
	{
		if (Manger.GetValue(TR_IS_GROUNDED, out bool _) && !Manger.GetValue(TR_DISABLE_SNAPPING, out bool _))
		{
			// Snap to the ground beneath us
			float feetPos = Body.position.y + (_feetPosition * transform.localScale.y);
			Body.position = Body.position.Add(y: (_groundHit.point.y - feetPos) / 2);
		}
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

		if (stats.floorCast.Visible)
		{
			Gizmos.color = stats.floorCast.Color;
			
			GetFloorRect(stats, out var center, out var size);
			Gizmos.DrawCube(center, size);
		}

		if (stats.roofCast.Visible)
		{
			Gizmos.color = stats.roofCast.Color;

			GetCeilingRect(stats, out var center, out var size);
			Gizmos.DrawCube(center, size);
		}
	}

	private void GetFloorRect(in PlayerMoveStats stats, out Vector2 center, out Vector2 size)
	{
		center = ((Vector2)transform.position) + stats.FloorDetectionArea.center;
		size = Vector2.Scale(stats.FloorDetectionArea.size, (Vector2)transform.localScale);
	}

	private void GetCeilingRect(in PlayerMoveStats stats, out Vector2 center, out Vector2 size)
	{
		center = (Vector2)transform.position + stats.CeilingDetectionArea.center;
		size = Vector2.Scale(stats.CeilingDetectionArea.size, (Vector2)transform.localScale);
	}
}
