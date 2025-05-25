using System;
using UnityEngine;

namespace Systems.Stats
{
	/// <summary>
	/// A basic stat modifier that applies a multiplier to a <see cref="float"/> or <see cref="int"/> value.
	/// </summary>
	[Serializable]
	public class StatMultiplier : IStatModifier<float>, IStatModifier<int>
	{
		[SerializeField] private bool _enabled = true;
		[SerializeField] private float _multiplier = 1f;

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
		/// The value by which to multiply the value.
		/// </summary>
		public float Multiplier {
			get => _multiplier;
			set {
				if (_multiplier != value)
				{
					_multiplier = value;
					OnValueChange?.Invoke();
				}
			}
		}

		public event Action OnValueChange;

		public StatMultiplier(float multiplier, bool enabled = true)
		{
			this.Multiplier = multiplier;
			this.Enabled = enabled;
		}

		public void Handle(ref StatQuery<float> query)
		{
			if (Enabled)
				query.Value *= Multiplier;
		}

		public void Handle(ref StatQuery<int> query)
		{
			if (Enabled)
				query.Value = (int)(query.Value * Multiplier);
		}

		public void OnAdd() { }

		public void OnRemove() { }
	}
}