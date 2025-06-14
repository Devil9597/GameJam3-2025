using System;
using UnityEngine;

public class PowerUp : TriggerEvent
{
	[Flags]
	public enum Abilities
	{
		All = -1,
		None = 0,
		Jump = 1,
		DoubleJump = Jump | 2,
		Grab = 4,
		Climb = 8,
		Dash = 16,
		Hover = 32,
	}

	public Abilities abilitiesUnlocked;
	private static CharacterController2D[] _players;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
		_players = FindObjectsByType<CharacterController2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
	}

	public void Start()
	{
		this.OnEnter += this.PowerUp_OnEnter;
	}

	public void OnDestroy()
	{
		this.OnEnter -= this.PowerUp_OnEnter;
	}

	private void PowerUp_OnEnter(GameObject arg0)
	{
		UnlockAbilities();
		Deactivate();
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public void UnlockAbilities()
	{
		if (abilitiesUnlocked is Abilities.None)
			return;

		foreach (var player in _players)
		{
			player.EnableAbilities(abilitiesUnlocked);
		}
	}
}
