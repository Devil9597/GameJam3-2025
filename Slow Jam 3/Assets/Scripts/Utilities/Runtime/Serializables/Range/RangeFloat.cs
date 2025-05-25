using System;
using UnityEngine;

namespace Utilities.Serializables
{
	[Serializable]
	public struct RangeFloat : IRange<float>, IEquatable<RangeFloat>
	{
		[SerializeField, Unity.Properties.DontCreateProperty] private float _min, _max;

		[Unity.Properties.CreateProperty]
		public float Min {
			readonly get => _min;
			set {
				_min = value;
				EnsureMinMax();
			}
		}
		[Unity.Properties.CreateProperty]
		public float Max {
			readonly get => _max;
			set {
				_max = value;
				EnsureMinMax();
			}
		}

		public RangeFloat(float min = 0, float max = 0)
		{
			_min = min;
			_max = max;
			EnsureMinMax();
		}

		public readonly float Clamp(in float value) => Mathf.Clamp(value, Min, Max);

		public readonly bool Contains(in float value, RangeInclusivity inclusivity = RangeInclusivity.Both)
		{
			return IsValueMoreThanMin(value, inclusivity) & IsValueLessThanMax(value, inclusivity);
		}

		public readonly float Lerp(float t) => Mathf.Lerp(Min, Max, t);

		public readonly float Loop(float value, RangeInclusivity inclusivity = RangeInclusivity.Min)
		{
			float diff = Max - Min;
			if (!IsValueMoreThanMin(value, inclusivity))
			{
				while (!IsValueMoreThanMin(value, inclusivity))
				{
					value += diff;
				}
			}
			else if (!IsValueLessThanMax(value, inclusivity))
			{
				while (!IsValueLessThanMax(value, inclusivity))
				{
					value -= diff;
				}
			}
			return value;
		}

		public readonly float Random(RangeInclusivity inclusivity = RangeInclusivity.Both)
		{
			float value = UnityEngine.Random.Range(Min, Max);
			while (!Contains(value, inclusivity))
			{
				value = UnityEngine.Random.Range(Min, Max);
			}
			return value;
		}

		#region Opertators
		public static implicit operator RangeFloat((float min, float max) range) => new(range.min, range.max);
		public static implicit operator (float, float)(RangeFloat range) => (range.Min, range.Max);

		public static implicit operator RangeFloat(Vector2 vector) => new(vector.x, vector.y);
		public static implicit operator Vector2(RangeFloat range) => new(range.Min, range.Max);

		public static explicit operator RangeInt(RangeFloat range) => new((int)range.Min, (int)range.Max);
		#endregion

		public readonly bool Equals(RangeFloat other) => this.Min == other.Min && this.Max == other.Max;
		public readonly override bool Equals(object obj) => obj is RangeFloat range && this.Equals(range);

		public override readonly int GetHashCode() => HashCode.Combine(Min, Max);

		public override readonly string ToString() => $"[{Min} - {Max}]";

		#region Helpers
		private void EnsureMinMax() => (_min, _max) = _max < _min ? (_max, _min) : (_min, _max);

		private readonly bool IsValueMoreThanMin(float value, RangeInclusivity inclusivity)
		{
			return inclusivity.HasFlag(RangeInclusivity.Min) ? value >= Min : value > Min;
		}

		private readonly bool IsValueLessThanMax(float value, RangeInclusivity inclusivity)
		{
			return inclusivity.HasFlag(RangeInclusivity.Max) ? value <= Max : value < Max;
		}
		#endregion
	}
}