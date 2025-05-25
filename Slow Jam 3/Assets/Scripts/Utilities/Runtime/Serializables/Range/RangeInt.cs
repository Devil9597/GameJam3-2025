using System;
using UnityEngine;

namespace Utilities.Serializables
{
	[Serializable]
	public struct RangeInt : IRange<int>, IEquatable<RangeInt>
	{
		[SerializeField, Unity.Properties.DontCreateProperty] private int _min, _max;

		[Unity.Properties.CreateProperty]
		public int Min {
			readonly get => _min;
			set {
				_min = value;
				EnsureMinMax();
			}
		}
		[Unity.Properties.CreateProperty]
		public int Max {
			readonly get => _max;
			set {
				_max = value;
				EnsureMinMax();
			}
		}

		public RangeInt(int min = 0, int max = 0)
		{
			_min = min;
			_max = max;
			EnsureMinMax();
		}

		public readonly int Clamp(in int value) => Mathf.Clamp(value, Min, Max);

		public readonly bool Contains(in int value, RangeInclusivity inclusivity = RangeInclusivity.Both)
		{
			return IsValueMoreThanMin(value, inclusivity) & IsValueLessThanMax(value, inclusivity);
		}

		public readonly int Lerp(float t) => (int)Mathf.Lerp(Min, Max, t);

		public readonly int Loop(int value, RangeInclusivity inclusivity = RangeInclusivity.Min)
		{
			int diff = Max - Min;
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

		public readonly int Random(RangeInclusivity inclusivity = RangeInclusivity.Both)
		{
			int min = inclusivity.HasFlag(RangeInclusivity.Min) ? Min : Min + 1;
			int max = inclusivity.HasFlag(RangeInclusivity.Max) ? Max + 1 : Max;
			return UnityEngine.Random.Range(min, max);
		}

		#region Operators
		public static implicit operator RangeInt((int min, int max) range) => new(range.min, range.max);
		public static implicit operator (int, int)(RangeInt range) => (range.Min, range.Max);

		public static implicit operator RangeInt(Vector2Int vector) => new(vector.x, vector.y);
		public static implicit operator Vector2Int(RangeInt range) => new(range.Min, range.Max);

		public static implicit operator RangeFloat(RangeInt range) => new(range.Min, range.Max);
		#endregion

		public readonly bool Equals(RangeInt other) => this.Min == other.Min && this.Max == other.Max;
		public override readonly bool Equals(object obj) => obj is RangeInt range && this.Equals(range);

		public override readonly int GetHashCode() => HashCode.Combine(Min, Max);

		public override readonly string ToString() => $"[{Min} - {Max}]";

		#region Helpers
		private void EnsureMinMax() => (_min, _max) = _max < _min ? (_max, _min) : (_min, _max);

		private readonly bool IsValueMoreThanMin(int value, RangeInclusivity inclusivity)
		{
			return inclusivity.HasFlag(RangeInclusivity.Min) ? value >= Min : value > Min;
		}

		private readonly bool IsValueLessThanMax(int value, RangeInclusivity inclusivity)
		{
			return inclusivity.HasFlag(RangeInclusivity.Max) ? value <= Max : value < Max;
		}
		#endregion
	}
}