using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RespawnableObject : MonoBehaviour, IRespawnable
{
	[SerializeField] private Vector2 _startPosition;
	[SerializeField] private bool _startActiveState;
	[SerializeField] private UnityEvent<int> _onRespawn;

	public event UnityAction<int> OnRespawn {
		add => _onRespawn.AddListener(value);
		remove => _onRespawn.RemoveListener(value);
	}

	IReadOnlyList<Vector2> IRespawnable.GetSpawnPositions()
	{
		return new Vector2[] { _startPosition };
	}

	public void Start()
	{
		_startPosition = transform.position;
		_startActiveState = true;
	}

	public void Respawn(int index = 0)
	{
		_onRespawn.Invoke(0);
		transform.position = _startPosition;
		gameObject.SetActive(_startActiveState);
	}

	void IRespawnable.SetSpawnPosition(Vector2 spawnPosition, int index)
	{
		_startPosition = spawnPosition;
	}
}
