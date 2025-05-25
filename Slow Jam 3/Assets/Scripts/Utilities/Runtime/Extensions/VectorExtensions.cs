using UnityEngine;

namespace Utilities.Extensions
{
	/// <summary>
	/// Class containing extension methods for vector types.
	/// </summary>
	public static class VectorExtensions
	{
		#region Add and With
		/// <summary>
		/// Replaces the components of a <see cref="Vector2"/> with the ones given.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Vector2 With(this Vector2 v, float? x = null, float? y = null)
		{
			return new(x ?? v.x, y ?? v.y);
		}
		/// <summary>
		/// Replaces the components of a <see cref="Vector2Int"/> with the ones given.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Vector2Int With(this Vector2Int v, int? x = null, int? y = null)
		{
			return new(x ?? v.x, y ?? v.y);
		}

		/// <summary>
		/// Adds the given components to the <see cref="Vector2"/>.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Vector2 Add(this Vector2 v, float? x = null, float? y = null)
		{
			v.x += x ?? 0;
			v.y += y ?? 0;
			return v;
		}
		/// <summary>
		/// Adds the given components to the <see cref="Vector2Int"/>.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Vector2Int Add(this Vector2Int v, int? x = null, int? y = null)
		{
			v.x += x ?? 0;
			v.y += y ?? 0;
			return v;
		}

		/// <summary>
		/// Replaces the components of a <see cref="Vector3"/> with the ones given.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
		{
			return new(x ?? v.x, y ?? v.y, z ?? v.z);
		}
		/// <summary>
		/// Replaces the components of a <see cref="Vector3Int"/> with the ones given.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public static Vector3Int With(this Vector3Int v, int? x = null, int? y = null, int? z = null)
		{
			return new(x ?? v.x, y ?? v.y, z ?? v.z);
		}

		/// <summary>
		/// Adds the given components to the <see cref="Vector3"/>.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public static Vector3 Add(this Vector3 v, float? x = null, float? y = null, float? z = null)
		{
			v.x += x ?? 0;
			v.y += y ?? 0;
			v.z += z ?? 0;
			return v;
		}
		/// <summary>
		/// Adds the given components to the <see cref="Vector3Int"/>.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public static Vector3Int Add(this Vector3Int v, int? x = null, int? y = null, int? z = null)
		{
			v.x += x ?? 0;
			v.y += y ?? 0;
			v.z += z ?? 0;
			return v;
		}

		/// <summary>
		/// Replaces the components of a <see cref="Vector4"/> with the ones given.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="w"></param>
		/// <returns></returns>
		public static Vector4 With(this Vector4 v, float? x = null, float? y = null, float? z = null, float? w = null)
		{
			return new(x ?? v.x, y ?? v.y, z ?? v.z, w ?? v.w);
		}

		/// <summary>
		/// Adds the given components to the <see cref="Vector4"/>.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="w"></param>
		/// <returns></returns>
		public static Vector4 Add(this Vector4 v, float? x = null, float? y = null, float? z = null, float? w = null)
		{
			v.x += x ?? 0;
			v.y += y ?? 0;
			v.z += z ?? 0;
			v.w += w ?? 0;
			return v;
		}
		#endregion

		/// <summary>
		/// Rotate the <see cref="Vector2"/> by the given <paramref name="angle"/> in degrees.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static Vector2 Rotate(this Vector2 v, float angle)
		{
			angle *= Mathf.Deg2Rad;
			v.x = v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle);
			v.y = v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle);
			return v;
		}

		/// <summary>
		/// <inheritdoc cref="Quaternion.Euler(float, float, float)"/>
		/// </summary>
		/// <param name="v"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public static Vector3 Rotate(this Vector3 v, float x = 0, float y = 0, float z = 0)
		{
			return Quaternion.Euler(x, y, z) * v;
		}

		/// <summary>
		/// Converts a <see cref="Vector2"/> into a <see cref="Vector3"/> with a y component of 0.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static Vector3 Flat(this Vector2 v) => new(v.x, 0, v.y);

		public static void Deconstruct(this Vector2 v, out float x, out float y)
		{
			x = v.x;
			y = v.y;
		}
		public static void Deconstruct(this Vector3 v, out float x, out float y, out float z)
		{
			x = v.x;
			y = v.y;
			z = v.z;
		}

		#region HVL
		/// <summary>
		/// Returns the x and y components of a <see cref="Vector3"/> as a <see cref="Vector2"/>, with the original z as the y.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static Vector2 Horizontal(this Vector3 v) => new(v.x, v.z);
		/// <summary>
		/// Returns the x and y components of a <see cref="Vector3"/> as a <see cref="Vector2"/>.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static Vector2 Vertical(this Vector3 v) => new(v.x, v.y);
		/// <summary>
		/// Returns the z and y components of a <see cref="Vector3"/> as a <see cref="Vector2"/>, with original z as the x.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static Vector2 Lateral(this Vector3 v) => new(v.z, v.y);

		/// <summary>
		/// Sets the x and z values to the given <paramref name="horizontal"/>'s x and y values, respectively.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="horizontal"></param>
		/// <returns></returns>
		public static Vector3 SetHorizontal(this Vector3 v, Vector2 horizontal)
		{
			return v.With(x: horizontal.x, z: horizontal.y);
		}
		/// <summary>
		/// Sets the x and y values to the given <paramref name="vertical"/>'s x and y values, respectively.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="vertical"></param>
		/// <returns></returns>
		public static Vector3 SetVertical(this Vector3 v, Vector2 vertical)
		{
			return v.With(x: vertical.x, y: vertical.y);
		}
		/// <summary>
		/// Sets the z and y values to the given <paramref name="lateral"/>'s x and y values, respectively.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="lateral"></param>
		/// <returns></returns>
		public static Vector3 SetLateral(this Vector3 v, Vector2 lateral)
		{
			return v.With(y: lateral.y, z: lateral.x);
		}
		#endregion

		/// <summary>
		/// Compares two <see cref="Vector2"/> values and returns <see langword="true"/> if they are similar.
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <returns></returns>
		public static bool Approximately(this Vector2 v1, Vector2 v2)
		{
			return Mathf.Approximately(v1.magnitude, v2.magnitude);
		}
		/// <summary>
		/// Compares two <see cref="Vector3"/> values and returns <see langword="true"/> if they are similar.
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <returns></returns>
		public static bool Approximately(this Vector3 v1, Vector3 v2)
		{
			return Mathf.Approximately(v1.magnitude, v2.magnitude);
		}
	}

	public static class NumericExtensions
	{
		public static float MoveTowards(this float f, float target, float delta)
		{
			if (f < target)
			{
				f += delta;
				f = f > target ? target : f;
			}
			else if (f > target)
			{
				f -= delta;
				f = f < target ? target : f;
			}
			return f;
		}
	}
}
