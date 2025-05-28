using System;
using UnityEngine;

namespace Systems.Stats
{
	/// <summary>
	/// Overrides the base and modified values.
	/// </summary>
	[Serializable]
	public class StatOverride<T> : IStatModifier<T> where T : struct
	{
		[SerializeField] private bool _enabled = false;
		[SerializeField] private T _newValue;

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

		public T NewValue {
			get => _newValue;
			set {
				_newValue = value;
				OnValueChange?.Invoke();
			}
		}

		public event Action OnValueChange;

		public StatOverride(T newValue, bool enabled = false)
		{
			_enabled = enabled;
			_newValue = newValue;
		}

		public void Handle(ref StatQuery<T> query)
		{
			if (Enabled)
			{
				query.BaseValue = NewValue;
				query.Value = NewValue;
			}
		}

		public void OnAdd()
		{
			// do nothing
		}

		public void OnRemove()
		{
			// do nothing
		}
	}
}
