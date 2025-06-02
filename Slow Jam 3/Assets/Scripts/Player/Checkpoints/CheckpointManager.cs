using UnityEngine;

public class CheckpointManager : Utilities.Singletons.RegulatorSingleton<CheckpointManager>
{
	[SerializeField] private Utilities.Serializables.InterfaceReference<IRespawnable> _player = new();
	public static IRespawnable Player => Instance._player.Value;

	protected override void Initialize()
	{
		if (_player.UnderlyingValue == null)
		{
			var player = GameObject.FindGameObjectWithTag("Player");
			if (player == null || !player.TryGetComponent(out IRespawnable component))
			{
				throw new System.Exception("No player found with a respawnable component.");
			}
			_player.UnderlyingValue = player;
			_player.Value = component;
		}

		Debug.Assert(_player != null);
	}
}
