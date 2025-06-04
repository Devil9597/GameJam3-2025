using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public void SavePlayerState()
	{
		CheckpointManager.Player.SaveState();
	}
}
