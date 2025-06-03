using level;
using UnityEngine;

public class Hazard : CollisionEvent
{
	public void OnEnable()
	{
		base.OnEnter += KillPlayer;
	}

	public void OnDisable()
	{
		base.OnEnter -= KillPlayer;
	}

	public static void KillPlayer(GameObject _)
	{
		CheckpointManager.Player.Respawn();
	}
}
