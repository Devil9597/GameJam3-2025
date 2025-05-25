using UnityEngine;

namespace Utilities.Serializables
{
	/// <summary>
	/// Allows for staggering the execution of expensive functions to improve performance.
	/// </summary>
	[System.Serializable]
	public struct TimeSlice
	{
		[Tooltip("Number of frames between executions.")]
		[SerializeField, Min(0)] private int _delay;
		[Tooltip("Offsets the current frame number by this amount.")]
		[SerializeField] private int _offset;

		/// <summary>
		/// Number of frames between executions. (Read only)
		/// </summary>
		public int Delay { readonly get => _delay; private set => _delay = value; }
		/// <summary>
		/// Offsets the current frame number by this amount.
		/// </summary>
		public int Offset { readonly get => _offset; set => _offset = value; }
		/// <summary>
		/// Increments every time <see cref="Update"/> is called. (Read only)
		/// </summary>
		public int Current { get; private set; }

		/// <summary>
		/// Creates a new <see cref="TimeSlice"/>.
		/// </summary>
		/// <param name="delay">Number of frames between executions. [0 - +inf.)</param>
		/// <param name="offset"></param>
		public TimeSlice(int delay, int offset = 0) : this()
		{
			Delay = delay < 0 ? 0 : delay;
			Offset = offset;
			Current = 0;
		}

		/// <summary>
		/// Increments <see cref="Current"/>.
		/// </summary>
		/// <param name="action">The <see cref="Action"/> to invoke if the current frame is valid.</param>
		/// <returns><see langword="true"/> if <see cref="Current"/> is inline with <see cref="Delay"/>, otherwise <see langword="false"/>.</returns>
		public bool Update(System.Action action = null)
		{
			Current++;

			if (Current > Delay + Offset)
			{
				Current = Offset;
				action?.Invoke();
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}