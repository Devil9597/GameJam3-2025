using System.Collections.Generic;
using System.Linq;
using Player.Checkpoints.Serialization;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Serializables;

/// <summary>
/// Objects with this script attached are <see cref="Respawnable"/> and can be moved any of the specified positions by calling the <see cref="Respawn(int)"/> method.
/// </summary>
public sealed class Respawnable : MonoBehaviour, IRespawnable
{
	[Tooltip("Event is invoked just before respawning.")]
	[SerializeField]
	private UnityEvent _onRespawn;
	[SerializeField] private List<SerializeGameObject> _managedObjects = new();

	/// <summary>
	/// Event is invoked just before moving to a spawn point.
	/// </summary>
	/// <remarks>
	/// The first parameter is the spawn point index.
	/// </remarks>
	public event UnityAction OnRespawn {
		add => _onRespawn.AddListener(value);
		remove => _onRespawn.RemoveListener(value);
	}

	private void Start()
	{
		_managedObjects =
			GameObject.FindObjectsByType<SerializeGameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None)
				.ToList();

		foreach (var obj in _managedObjects)
		{
			obj.SaveState();
		}
	}

	public void SaveState()
	{
		foreach (var checkpointAwareObject in _managedObjects)
		{
			checkpointAwareObject.SaveState();
		}
	}

	public void Respawn()
	{
		_onRespawn?.Invoke();

		foreach (var checkpointAwareObject in _managedObjects)
		{
			checkpointAwareObject.LoadState();
		}
	}
}

public static class RespawnableHelpers
{
	public enum ShowGizmoMode
	{
		Hidden,
		Selected,
		Always
	}

	public static bool ContainsIndex<T>(this IList<T> array, int index)
	{
		return array is not null && (0, array.Count).Contains(index, RangeInclusivity.Min);
	}
}