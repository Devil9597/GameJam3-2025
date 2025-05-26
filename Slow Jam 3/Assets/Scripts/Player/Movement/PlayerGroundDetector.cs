using UnityEngine;
using Utilities.Serializables;

public class PlayerGroundDetector : PlayerMotionController
{
	[SerializeField] private Vector2 _castOrigin;
	[SerializeField] private TimeSlice _castingPeriod;

	private RaycastHit2D _groundHit;

	public override void BeforeMovement()
	{
		if (_castingPeriod.Update())
		{
			GetRect(Stats, out var center, out var size);
			_groundHit = Physics2D.BoxCast(center, size, 0, Vector2.zero, 0, Stats.GroundLayer);

			Manger.isGrounded = _groundHit.collider != null;
		}
	}

	public override void ApplyMovement(in float deltaTime)
	{
		// nop
	}

	public override void Initialize()
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
			GetRect(stats, out var center, out var size);
			Gizmos.DrawCube(center, size);
		}
	}

	private void GetRect(in PlayerMoveStats stats, out Vector2 center, out Vector3 size)
	{
		center = ((Vector2)transform.position) + _castOrigin + stats.GroundDetectionArea.center;
		size = Vector2.Scale(stats.GroundDetectionArea.size, (Vector2)transform.localScale);
	}
}
