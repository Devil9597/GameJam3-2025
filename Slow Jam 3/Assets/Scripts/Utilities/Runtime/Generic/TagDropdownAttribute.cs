using UnityEngine;

namespace Utilities
{
	/// <summary>
	/// Use this <see cref="PropertyAttribute"/> to make a string field appear as a tag dropdown.
	/// </summary>
	public class TagDropdownAttribute : PropertyAttribute
	{
		public bool hideLabel = false;

		public TagDropdownAttribute(bool hideLabel = false) => this.hideLabel = hideLabel;
	}
}
