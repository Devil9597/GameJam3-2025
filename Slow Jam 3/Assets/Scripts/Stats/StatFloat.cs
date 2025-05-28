using System;
using UnityEngine;

namespace Systems.Stats
{
	[Serializable]
	public class StatFloat : StatValue<float>
	{
		[SerializeField] private float _baseValue;

		public override float BaseValue { get => _baseValue; set { _baseValue = value; base.BaseValue = value; } }

		public StatFloat() : base() { }
		public StatFloat(float baseValue) : base(baseValue) { }
		public StatFloat(float baseValue, params IStatModifier<float>[] modifiers) : base(baseValue, modifiers) { }

		public static implicit operator StatFloat(float baseValue) {  return new StatFloat(baseValue); }
	}
}