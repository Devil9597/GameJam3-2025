using System;
using UnityEngine;

namespace Systems.Stats
{
	[Serializable]
	public class StatInt : StatValue<int>
	{
		[SerializeField] private int _baseValue;

		public override int BaseValue { get => _baseValue; set { _baseValue = value; base.BaseValue = value; } }

		public StatInt() : base() { }
		public StatInt(int baseValue) : base(baseValue) { }
		public StatInt(int baseValue, params IStatModifier<int>[] modifiers) : base(baseValue, modifiers) { }

		public static implicit operator StatInt(int baseValue) { return new StatInt(baseValue); }
	}
}