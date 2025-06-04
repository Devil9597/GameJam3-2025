using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IRespawnable
{
	event UnityAction OnRespawn;

	void Respawn();
	void SaveState();
}