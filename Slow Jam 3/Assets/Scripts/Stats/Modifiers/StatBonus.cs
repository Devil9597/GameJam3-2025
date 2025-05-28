using System;
using UnityEngine;

namespace Systems.Stats
{
	/// <summary>
	/// A basic stat modifier that adds a percentage of the base value to the result.
	/// </summary>
	[Serializable]
	public sealed class StatBonus : IStatModifier<float>, IStatModifier<int>
	{
		[SerializeField] private bool _enabled = true;
		[SerializeField] private int _percentage;

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
		/// The percentage of the base value to increase the final value by.
		/// </summary>
		public int Percentage {
			get => _percentage;
			set {
				if (_percentage != value)
				{
					_percentage = value;
					OnValueChange?.Invoke();
				}
			}
		}

		public event Action OnValueChange;

		public void Handle(ref StatQuery<float> query)
		{
			if (!Enabled) return;
			query.Value += query.BaseValue * (1 + (Percentage / 100f));
		}

		public void Handle(ref StatQuery<int> query)
		{
			if (!Enabled) return;
			query.Value += (int)(query.BaseValue * (1 + (Percentage / 100f)));
		}

		public void OnAdd() { }

		public void OnRemove() { }
	}
}