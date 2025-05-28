using System;

namespace Systems.Stats
{
	public interface IStatModifier<T> where T : struct
	{
		/// <summary>
		/// Invoke this event after changes are made that would require recalculation.
		/// </summary>
		event Action OnValueChange;

		/// <summary>
		/// Handles the modification of the <see cref="StatQuery{T}"/> value.
		/// </summary>
		/// <param name="query"></param>
		void Handle(ref StatQuery<T> query);
		/// <summary>
		/// This method is invoked whenever this modifier is added to a <see cref="StatValue{T}"/>.
		/// </summary>
		void OnAdd();
		/// <summary>
		/// This method is invoked whenever this modifier is removed from a <see cref="StatValue{T}"/>.
		/// </summary>
		void OnRemove();
	}
}