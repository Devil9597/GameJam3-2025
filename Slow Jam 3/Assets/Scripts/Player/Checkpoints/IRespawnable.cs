using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IRespawnable
{
	event UnityAction<int> OnRespawn;

	IReadOnlyList<Vector2> GetSpawnPositions();
	void Respawn(int index = 0);
	void SetSpawnPosition(Vector2 spawnPosition, int index = 0);
}