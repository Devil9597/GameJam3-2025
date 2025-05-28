using System;
using UnityEngine;
using Utilities.Serializables;

using RangeInt = Utilities.Serializables.RangeInt;

namespace Systems.Stats
{
	/// <summary>
	/// Modifier that clamps a <see cref="float"/> or <see cref="int"/> value between a min and a max.
	/// </summary>
	[Serializable]
	public class StatClamp : IStatModifier<float>, IStatModifier<int>
	{
		[SerializeField] private bool _enabled = true;
		[SerializeField] private RangeFloat _range;

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

		public RangeFloat Range => _range;

		public float Min {
			get => _range.Min;
			set {
				if (_range.Min != value)
				{
					_range.Min = value;
					OnValueChange?.Invoke();
				}
			}
		}

		public float Max {
			get => _range.Max;
			set {
				if (_range.Max != value)
				{
					_range.Max = value;
					OnValueChange?.Invoke();
				}
			}
		}

		public event Action OnValueChange;

		public StatClamp(RangeFloat range, bool enabled = true)
		{
			_enabled = enabled;
			_range = range;
		}

		public StatClamp(float min, float max, bool enabled = true)
		{
			_enabled = enabled;
			_range = new RangeFloat(min, max);
		}

		public void Handle(ref StatQuery<float> query)
		{
			if (Enabled)
				query.Value = Range.Clamp(query.Value);
		}

		public void Handle(ref StatQuery<int> query)
		{
			if (Enabled)
				query.Value = ((RangeInt)Range).Clamp(query.Value);
		}

		public void OnAdd() { }

		public void OnRemove() { }
	}
}