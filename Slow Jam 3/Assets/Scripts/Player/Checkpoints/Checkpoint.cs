using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	[Tooltip("The target spawnpoint index of the player's respawnable component.")]
	[SerializeField, Min(0)] private int _targetIndex = 0;

	public void SetAsPlayerSpawnPoint()
	{
		CheckpointManager.Player.SetSpawnPosition(transform.position, _targetIndex);
	}
}
