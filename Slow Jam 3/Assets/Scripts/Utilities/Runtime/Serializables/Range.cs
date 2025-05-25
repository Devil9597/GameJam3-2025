using System;
using UnityEngine;

namespace Utilities.Serializables
{
	[Flags] public enum RangeInclusivity { None = 0, Min = 1, Max = 2, Both = Min | Max }

	[Serializable]
	public struct RangeInt
	{
		[SerializeField] private int _min, _max;

		public int Min { readonly get => _min; set => _min = value; }
		public int Max { readonly get => _max; set => _max = value; }

		public RangeInt(int min = 0, int max = 0) => (_min, _max) = max < min ? (max, min) : (min, max);

		public readonly bool Contains(float value, RangeInclusivity inclusivity = RangeInclusivity.Both)
		{
			bool contains = inclusivity.HasFlag(RangeInclusivity.Min) ? Min <= value : Min < value;
			contains &= inclusivity.HasFlag(RangeInclusivity.Max) ? Max >= value : Max > value;
			return contains;
		}

		public static implicit operator Vector2Int(RangeInt range) => new(x: range.Min, y: range.Max);
		public static implicit operator RangeInt(Vector2Int vector2) => new(min: vector2.x, max: vector2.y);

		public static implicit operator Vector2(RangeInt range) => new(x: range.Min, y: range.Max);
		public static explicit operator RangeInt(Vector2 vector2) => new(min: (int)vector2.x, max: (int)vector2.y);

		public static implicit operator RangeFloat(RangeInt range) => new(min: range.Min, max: range.Max);

		public static implicit operator RangeInt((int, int) values) => new(min: values.Item1, max: values.Item2);

		public override readonly bool Equals(object obj) => obj is RangeInt range &&
				this.Min == range.Min && this.Max == range.Max;

		public override readonly int GetHashCode() => HashCode.Combine(Min, Max);

		public override readonly string ToString() => $"[{Min} - {Max}]";

		public readonly Vector2Int Cast() => this;
	}

	[Serializable]
	public struct RangeFloat
	{
		[SerializeField] private float _min, _max;

		public float Min { readonly get => _min; set => _min = value; }
		public float Max { readonly get => _max; set => _max = value; }

		public RangeFloat(float min = 0, float max = 0) => (_min, _max) = max < min ? (max, min) : (min, max);

		public readonly bool Contains(float value, RangeInclusivity inclusivity = RangeInclusivity.Both)
		{
			bool contains = inclusivity.HasFlag(RangeInclusivity.Min) ? Min <= value : Min < value;
			contains &= inclusivity.HasFlag(RangeInclusivity.Max) ? Max >= value : Max > value;
			return contains;
		}

		public readonly float Lerp(float t) => Mathf.Lerp(Min, Max, t);

		public static implicit operator Vector2(RangeFloat range) => new(x: range.Min, y: range.Max);
		public static implicit operator RangeFloat(Vector2 vector2) => new(min: vector2.x, max: vector2.y);

		public static implicit operator RangeFloat(Vector2Int vector2) => new(min: vector2.x, max: vector2.y);
		public static explicit operator Vector2Int(RangeFloat range) => new(x: (int)range.Min, y: (int)range.Max);

		public static explicit operator RangeInt(RangeFloat range) => new(min: (int)range.Min, max: (int)range.Max);

		public static implicit operator RangeFloat((float, float) values) => new(min: values.Item1, max: values.Item2);

		public readonly override bool Equals(object obj) => obj is RangeFloat range &&
				this.Min == range.Min && this.Max == range.Max;

		public override readonly int GetHashCode() => HashCode.Combine(Min, Max);

		public override readonly string ToString() => $"[{Min} - {Max}]";
	}

	public static class RangeExtensions
	{
		public static bool Contains(this (int min, int max) range, float value, RangeInclusivity inclusivity = RangeInclusivity.Both)
		{
			return new RangeInt(min: range.min, max: range.max).Contains(value, inclusivity);
		}

		public static bool Contains(this (float min, float max) range, float value, RangeInclusivity inclusivity = RangeInclusivity.Both)
		{
			return new RangeFloat(min: range.min, max: range.max).Contains(value, inclusivity);
		}

		public static int Random(this (int min, int max) range, bool maxInclusive = false) => UnityEngine.Random.Range(range.min, range.max + (maxInclusive ? 1 : 0));
		public static int Random(this RangeInt range, bool maxInclusive = false) => UnityEngine.Random.Range(range.Min, range.Max + (maxInclusive ? 1 : 0));

		public static float Random(this (float min, float max) range) => UnityEngine.Random.Range(range.min, range.max);
		public static float Lerp(this (float min, float max) range, float t) => Mathf.Lerp(range.min, range.max, t);
		public static float Random(this RangeFloat range) => UnityEngine.Random.Range(range.Min, range.Max);
	}
}