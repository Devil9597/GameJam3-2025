using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.Serializables
{
	[Serializable]
	public class InterfaceReference<TInterface, TObject>
		where TObject : Object
		where TInterface : class
	{
		[SerializeField, HideInInspector] private TObject _underlyingValue;

		public TInterface Value {
			get => _underlyingValue switch {
				null => null,
				TInterface @interface => @interface,
				_ => throw new InvalidOperationException($"{_underlyingValue} does not implement interface {nameof(TInterface)}."),
			};
			set => _underlyingValue = value switch {
				null => null,
				TObject newValue => newValue,
				_ => throw new ArgumentException($"{value} must be of type {typeof(TObject)}.", string.Empty),
			};
		}

		public TObject UnderlyingValue {
			get => _underlyingValue;
			set => _underlyingValue = value;
		}

		public InterfaceReference() { }

		public InterfaceReference(TObject target) => _underlyingValue = target;

		public InterfaceReference(TInterface @interface) => _underlyingValue = @interface as TObject;
	}

	[Serializable]
	public class InterfaceReference<TInterface> : InterfaceReference<TInterface, Object> where TInterface : class { }
}
