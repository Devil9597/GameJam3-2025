using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Stats
{
	public interface IStatValue<T> where T : struct
	{
		/// <summary>
		/// The base <typeparamref name="T"/> value without any modifiers.
		/// </summary>
		T BaseValue { get; }
		/// <summary>
		/// The <typeparamref name="T"/> value after all modifier have been applied.
		/// </summary>
		T ModifiedValue { get; }
		/// <summary>
		/// List of modifiers on this value.
		/// </summary>
		IReadOnlyList<IStatModifier<T>> Modifiers { get; }

		/// <summary>
		/// Use this method request a recalculation of the modified value.
		/// </summary>
		void SetDirty();
		/// <summary>
		/// Add a modifier to the list of modifiers.
		/// </summary>
		/// <param name="modifier">The <see cref="IStatModifier{T}"/> to add.</param>
		void AddModifier(IStatModifier<T> modifier);
		/// <summary>
		/// Removes a modifier from the list of modifiers.
		/// </summary>
		/// <param name="modifier">The <see cref="IStatModifier{T}"/> to remove.</param>
		void RemoveModifier(IStatModifier<T> modifier);
		/// <summary>
		/// Removes all modifiers.
		/// </summary>
		void ClearModifiers();
	}

	/// <summary>
	/// Base class for <typeparamref name="T"/> variables that can use modifiers.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StatValue<T> : IStatValue<T> where T : struct
	{
		protected bool _dirty;
		protected T _cachedValue;

		[NonSerialized] private T _baseValue;
		[NonSerialized] protected readonly List<IStatModifier<T>> _modifiers;

		protected StatQuery<T>.Delegate _query;

		public virtual T BaseValue { get => _baseValue; set { _baseValue = value; SetDirty(); } }
		public virtual T ModifiedValue => _dirty ? GetValue() : _cachedValue;
		public virtual IReadOnlyList<IStatModifier<T>> Modifiers => _modifiers;

		public StatValue()
			: this(baseValue: default)
		{ }
		public StatValue(T baseValue)
			: this(baseValue, Array.Empty<IStatModifier<T>>())
		{ }
		public StatValue(T baseValue, params IStatModifier<T>[] modifiers)
			: this(baseValue, (IEnumerable<IStatModifier<T>>)modifiers)
		{ }
		public StatValue(T baseValue, IEnumerable<IStatModifier<T>> modifiers)
		{
			_dirty = true;
			BaseValue = baseValue;
			_modifiers = new();
			foreach (var modifier in modifiers)
			{
				AddModifier(modifier);
			}
		}

		~StatValue()
		{
			for (int i = Modifiers.Count - 1; i >= 0; i--)
			{
				RemoveModifier(Modifiers[i]);
			}
		}

		public void SetDirty() => _dirty = true;

		protected virtual T GetValue()
		{
			var q = new StatQuery<T>(BaseValue);
			_query?.Invoke(ref q);
			_cachedValue = q.Value;
			_dirty = false;
			return _cachedValue;
		}

		public virtual void AddModifier(IStatModifier<T> modifier)
		{
			if (!_modifiers.Contains(modifier))
			{
				SetDirty();
				_modifiers.Add(modifier);
				_query += modifier.Handle;
				modifier.OnValueChange += SetDirty;
				modifier.OnAdd();
			}
			else
			{
				Debug.LogWarning("Modifier has already been added!");
			}
		}

		public virtual void RemoveModifier(IStatModifier<T> modifier)
		{
			if (_modifiers.Contains(modifier))
			{
				SetDirty();
				_modifiers.Remove(modifier);
				_query -= modifier.Handle;
				modifier.OnValueChange -= SetDirty;
				modifier.OnRemove();
			}
		}

		public virtual void ClearModifiers()
		{
			_modifiers.Clear();
			SetDirty();
		}
	}
}