using System;

namespace Utilities.Serializables
{
	/// <summary>
	/// Flags for specifying a range's inclusivity for some methods.
	/// </summary>
	[Flags]
	public enum RangeInclusivity
	{
		/// <summary>Nether the minimum or the maximum are included.</summary>
		None = 0,
		/// <summary>The minimum is included.</summary>
		Min = 1,
		/// <summary>The maximum is included.</summary>
		Max = 2,
		/// <summary>Both minimum and maximum are included.</summary>
		Both = Min | Max
	}

	/// <summary>
	/// Extensions class so that int and float tuples can be treated as ranges.
	/// </summary>
	public static class RangeExtensions
	{
		#region Int Range
		/// <summary>
		/// Converts an pair of <see cref="int"/>s into a <see cref="RangeInt"/>.
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public static RangeInt ToRange(this (int min, int max) range) => range;

		/// <summary>
		/// <inheritdoc cref="RangeInt.Clamp(in int)"/>
		/// </summary>
		/// <param name="range"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int Clamp(this (int min, int max) range, in int value) => range.ToRange().Clamp(value);
		/// <summary>
		/// <inheritdoc cref="RangeInt.Contains(in int, RangeInclusivity)"/>
		/// </summary>
		/// <param name="range"></param>
		/// <param name="value"></param>
		/// <param name="inclusivity"></param>
		/// <returns></returns>
		public static bool Contains(this (int min, int max) range, in int value, RangeInclusivity inclusivity) => range.ToRange().Contains(value, inclusivity);
		/// <summary>
		/// <inheritdoc cref="RangeInt.Lerp(float)"/>
		/// </summary>
		/// <param name="range"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static int Lerp(this (int min, int max) range, float t) => range.ToRange().Lerp(t);
		/// <summary>
		/// <inheritdoc cref="RangeInt.Loop(int, RangeInclusivity)"/>
		/// </summary>
		/// <param name="range"></param>
		/// <param name="value"></param>
		/// <param name="inclusivity"></param>
		/// <returns></returns>
		public static int Loop(this (int min, int max) range, int value, RangeInclusivity inclusivity = RangeInclusivity.Min) => range.ToRange().Loop(value, inclusivity);
		/// <summary>
		/// <inheritdoc cref="RangeInt.Random(RangeInclusivity)"/>
		/// </summary>
		/// <param name="range"></param>
		/// <param name="inclusivity"></param>
		/// <returns></returns>
		public static int Random(this (int min, int max) range, RangeInclusivity inclusivity = RangeInclusivity.Both) => range.ToRange().Random(inclusivity);
		#endregion

		#region Float Range
		/// <summary>
		/// Converts a pair of <see cref="float"/>s into a <see cref="RangeFloat"/>.
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public static RangeFloat ToRange(this (float min, float max) range) => range;

		/// <summary>
		/// <inheritdoc cref="RangeFloat.Clamp(in float)"/>
		/// </summary>
		/// <param name="range"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static float Clamp(this (float min, float max) range, in float value) => range.ToRange().Clamp(value);
		/// <summary>
		/// <inheritdoc cref="RangeFloat.Contains(in float, RangeInclusivity)"/>
		/// </summary>
		/// <param name="range"></param>
		/// <param name="value"></param>
		/// <param name="inclusivity"></param>
		/// <returns></returns>
		public static bool Contains(this (float min, float max) range, in float value, RangeInclusivity inclusivity = RangeInclusivity.Both) => range.ToRange().Contains(value, inclusivity);
		/// <summary>
		/// <inheritdoc cref="RangeFloat.Lerp(float)"/>
		/// </summary>
		/// <param name="range"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float Lerp(this (float min, float max) range, float t) => range.ToRange().Lerp(t);
		/// <summary>
		/// <inheritdoc cref="RangeFloat.Loop(float, RangeInclusivity)"/>
		/// </summary>
		/// <param name="range"></param>
		/// <param name="value"></param>
		/// <param name="inclusivity"></param>
		/// <returns></returns>
		public static float Loop(this (float min, float max) range, float value, RangeInclusivity inclusivity = RangeInclusivity.Min) => range.ToRange().Loop(value, inclusivity);
		/// <summary>
		/// <inheritdoc cref="RangeFloat.Random(RangeInclusivity)"/>
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public static float Random(this (float min, float max) range, RangeInclusivity inclusivity = RangeInclusivity.Both) => range.ToRange().Random(inclusivity);
		#endregion
	}
}