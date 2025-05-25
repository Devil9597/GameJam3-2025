using System;
using UnityEngine;

namespace Utilities.Serializables
{
	[AttributeUsage(AttributeTargets.Field)]
	public class RequireInterfaceAttribute : PropertyAttribute
	{
		public readonly Type interfaceType;

		public RequireInterfaceAttribute(Type interfaceType)
		{
			Debug.Assert(interfaceType.IsInterface, $"{nameof(interfaceType)} must be an interface.");
			this.interfaceType = interfaceType;
		}
	}
}