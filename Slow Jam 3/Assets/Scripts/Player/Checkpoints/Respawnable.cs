using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Serializables;

/// <summary>
/// Objects with this script attached are <see cref="Respawnable"/> and can be moved any of the specified positions by calling the <see cref="Respawn(int)"/> method.
/// </summary>
public sealed class Respawnable : MonoBehaviour, IRespawnable
{
	[Tooltip("Will this object be respawned when the game starts?")]
	[SerializeField] private bool _respawnOnStart = true;
	[Tooltip("List of spawn points this object can be respawned at.")]
	[SerializeField] private Transform[] _spawnPoints = new Transform[1];
	[Tooltip("Event is invoked just before respawning.")]
	[SerializeField] private UnityEvent<int> _onRespawn;

	[Header("Debug Settings")]
	[SerializeField] private Color _gizmoColor = Color.white;
	[SerializeField] private RespawnableHelpers.ShowGizmoMode _showConnections = RespawnableHelpers.ShowGizmoMode.Selected;

	/// <summary>
	/// Event is invoked just before moving to a spawn point.
	/// </summary>
	/// <remarks>
	/// The first parameter is the spawn point index.
	/// </remarks>
	public event UnityAction<int> OnRespawn {
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

	/// <summary>
	/// Creates a list of all positions in the spawn points list.
	/// </summary>
	/// <returns></returns>
	public IReadOnlyList<Vector2> GetSpawnPositions()
	{
		return _spawnPoints.Select(sp => (Vector2)sp.position).ToArray();
	}

	/// <summary>
	/// Sets the position of the spawn point at the given <paramref name="index"/>.
	/// </summary>
	/// <param name="spawnPosition"></param>
	/// <param name="index"></param>
	public void SetSpawnPosition(Vector2 spawnPosition, int index = 0)
	{
		if (!this.HasSpawnPoint(index)) return;

		_spawnPoints[index].position = spawnPosition;
	}

	/// <summary>
	/// Moves this object's transform to the spawn point at the given <paramref name="index"/>.
	/// </summary>
	/// <param name="index"></param>
	public void Respawn(int index = 0)
	{
		if (!this.HasSpawnPoint(index)) return;

		_onRespawn?.Invoke(index);
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
}

public static class RespawnableHelpers
{
	public enum ShowGizmoMode { Hidden, Selected, Always }

	public static bool ContainsIndex<T>(this IList<T> array, int index)
	{
		return array is not null && (0, array.Count).Contains(index, RangeInclusivity.Min);
	}
}
