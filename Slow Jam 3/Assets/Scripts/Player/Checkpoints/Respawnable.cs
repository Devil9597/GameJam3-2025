using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Serializables;

public sealed class Respawnable : MonoBehaviour
{
	[SerializeField] private bool _respawnOnStart = true;
	[SerializeField] private Transform[] _spawnPoints = new Transform[1];
	[SerializeField] private UnityEvent _onRespawn;

#if UNITY_EDITOR
	[Header("Debug Settings")]
	[SerializeField] private RespawnableHelpers.ShowGizmoMode _showConnections = RespawnableHelpers.ShowGizmoMode.Always;
	[SerializeField] private Color _gizmoColor = Color.white;
#endif

	public event UnityAction OnRespawn {
		add => _onRespawn.AddListener(value);
		remove => _onRespawn.RemoveListener(value);
	}

	private void Start()
	{
		if (_respawnOnStart)
		{
			this.Respawn();
		}
	}

	public IReadOnlyList<Vector2> GetSpawnPositions()
	{
		return _spawnPoints.Select(sp => (Vector2)sp.position).ToArray();
	}

	public void SetSpawnPosition(Vector2 spawnPosition, int index = 0)
	{
		if (!this.HasSpawnPoint(index)) return;

		_spawnPoints[index].position = spawnPosition;
	}

	public void Respawn(int index = 0)
	{
		if (!this.HasSpawnPoint(index)) return;

		_onRespawn?.Invoke();
		transform.position = _spawnPoints[index].position;
	}

	private bool HasSpawnPoint(int index = 0)
	{
		var hasSpawnPoint = _spawnPoints.ContainsIndex(index) && (bool)_spawnPoints[index];
		if (!hasSpawnPoint)
		{
			Debug.LogError($"The object {name} has no spawn point at index {index}.");
		}
		return hasSpawnPoint;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (_showConnections is not RespawnableHelpers.ShowGizmoMode.Always)
			return;

		DrawConnections();
	}

	private void OnDrawGizmosSelected()
	{
		if (_showConnections is not RespawnableHelpers.ShowGizmoMode.Selected)
			return;

		DrawConnections();
	}

	private void DrawConnections()
	{
		Gizmos.color = _gizmoColor;

		for (int i = 0; i < _spawnPoints.Length; i++)
		{
			if (_spawnPoints[i] != null)
			{
				Gizmos.DrawLine(transform.position, _spawnPoints[i].position);
			}
		}
	}
#endif
}

public static class RespawnableHelpers
{
#if UNITY_EDITOR
	public enum ShowGizmoMode { Hidden, Selected, Always }
#endif

	public static bool ContainsIndex<T>(this IList<T> array, int index)
	{
		return array is not null && (0, array.Count).Contains(index, RangeInclusivity.Min);
	}
}
