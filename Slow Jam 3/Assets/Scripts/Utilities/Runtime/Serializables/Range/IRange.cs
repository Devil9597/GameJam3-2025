namespace Utilities.Serializables
{
	/// <summary>
	/// Base interface for a range of <typeparamref name="T"/> values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRange<T> where T : struct
	{
		/// <summary>
		/// The range's maximum value.
		/// </summary>
		T Max { get; set; }
		/// <summary>
		/// The range's minimum value.
		/// </summary>
		T Min { get; set; }

		/// <summary>
		/// Clamps the given <paramref name="value"/> to be no greater than max and no lesser than min.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		T Clamp(in T value);

		/// <summary>
		/// Returns <see langword="true"/> if the given <typeparamref name="T"/> <paramref name="value"/> is within range.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="inclusivity">Whether the min and max values are considered as part of the range.</param>
		/// <returns></returns>
		bool Contains(in T value, RangeInclusivity inclusivity = RangeInclusivity.Both);

		/// <summary>
		/// Interpolates between the min and max values using the given value <paramref name="t"/>, where 0 returns min and 1 returns max.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		T Lerp(float t);

		/// <summary>
		/// Loops the given <paramref name="value"/>, so that it is never greater than max or lesser than min.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="inclusivity">Whether the min and max values are considered as part of the range.</param>
		/// <returns></returns>
		T Loop(T value, RangeInclusivity inclusivity = RangeInclusivity.Min);

		/// <summary>
		/// Returns A random value contained within the range.
		/// </summary>
		/// <returns></returns>
		T Random(RangeInclusivity inclusivity = RangeInclusivity.Both);
	}
}