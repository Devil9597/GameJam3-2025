using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.Serializables
{
	[Serializable]
	public class Dictionary<TKey, TValue> : ISerializationCallbackReceiver, IDictionary<TKey, TValue>
	{
		[NonSerialized] private System.Collections.Generic.Dictionary<TKey, TValue> _underlyingValue = new();
		[SerializeField] private KeyValuePair<TKey, TValue>[] _keyValuePairs;

		public TValue this[TKey key] { get => ((IDictionary<TKey, TValue>)_underlyingValue)[key]; set => ((IDictionary<TKey, TValue>)_underlyingValue)[key] = value; }

		public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_underlyingValue).Keys;

		public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_underlyingValue).Values;

		public int Count => ((ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)_underlyingValue).Count;

		public bool IsReadOnly => ((ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)_underlyingValue).IsReadOnly;

		public void Add(TKey key, TValue value)
		{
			((IDictionary<TKey, TValue>)_underlyingValue).Add(key, value);
		}

		public void Add(System.Collections.Generic.KeyValuePair<TKey, TValue> item)
		{
			((ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)_underlyingValue).Add(item);
		}

		public void Clear()
		{
			((ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)_underlyingValue).Clear();
		}

		public bool Contains(System.Collections.Generic.KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)_underlyingValue).Contains(item);
		}

		public bool ContainsKey(TKey key)
		{
			return ((IDictionary<TKey, TValue>)_underlyingValue).ContainsKey(key);
		}

		public void CopyTo(System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)_underlyingValue).CopyTo(array, arrayIndex);
		}

		public IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return ((IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>)_underlyingValue).GetEnumerator();
		}

		public void OnAfterDeserialize()
		{
			IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> cast()
			{
				for (int i = 0; i < _keyValuePairs.Length; i++)
				{
					yield return _keyValuePairs[i];
				}
			}
			_underlyingValue = new(collection: cast());
		}


		public void OnBeforeSerialize()
		{
			// no-op
		}

		public bool Remove(TKey key)
		{
			return ((IDictionary<TKey, TValue>)_underlyingValue).Remove(key);
		}

		public bool Remove(System.Collections.Generic.KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)_underlyingValue).Remove(item);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return ((IDictionary<TKey, TValue>)_underlyingValue).TryGetValue(key, out value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_underlyingValue).GetEnumerator();
		}
	}

	[Serializable]
	public struct KeyValuePair<TKey, TValue>
	{
		[field: SerializeField] public TKey Key { get; set; }
		[field: SerializeField] public TValue Value { get; set; }

		public KeyValuePair(TKey key, TValue value)
		{
			this.Key = key;
			this.Value = value;
		}

		public static implicit operator System.Collections.Generic.KeyValuePair<TKey, TValue>(KeyValuePair<TKey, TValue> obj)
		{
			return new(obj.Key, obj.Value);
		}

		public static implicit operator KeyValuePair<TKey, TValue>(System.Collections.Generic.KeyValuePair<TKey, TValue> obj)
		{
			return new(obj.Key, obj.Value);
		}
	}
}
