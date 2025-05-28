using System;
using UnityEngine;

namespace Systems.Stats
{
	/// <summary>
	/// Subtracts a flat amount from a <see cref="float"/> or <see cref="int"/> value.
	/// </summary>
	[Serializable]
	public sealed class StatDecrease : IStatModifier<float>, IStatModifier<int>
	{
		[SerializeField] private bool _enabled;
		[SerializeField] private float _amount;

		/// <summary>
		/// Should this modifier be applied?
		/// </summary>
		public bool Enabled {
			get => _enabled;
			set {
				if (_enabled != value)
				{
					_enabled = value;
					OnValueChange?.Invoke();
				}
			}
		}

		/// <summary>
		/// The amount to increase the final value by.
		/// </summary>
		public float Amount {
			get => _amount;
			set {
				if (_amount != value)
				{
					_amount = value;
					OnValueChange?.Invoke();
				}
			}
		}

		public event Action OnValueChange;

		public StatDecrease(int amount, bool enabled = true)
		{
			Amount = amount;
			Enabled = enabled;
		}

		public void Handle(ref StatQuery<int> query)
		{
			if (!Enabled) return;
			query.Value -= (int)Amount;
		}

		public void Handle(ref StatQuery<float> query)
		{
			if (!Enabled) return;
			query.Value -= (float)Amount;
		}

		public void OnAdd() { }

		public void OnRemove() { }
	}
}