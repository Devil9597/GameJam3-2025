using UnityEditor;
using UnityEditor.UIElements;

namespace Utilities.Editor
{
	[CustomPropertyDrawer(typeof(TagDropdownAttribute))]
	public class TagDropdownAttributeDrawer : PropertyDrawer
	{
		public const string DEFAULT_TAG = "Untagged";

		public override UnityEngine.UIElements.VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var attr = attribute as TagDropdownAttribute;
			string label = attr.hideLabel ? string.Empty : preferredLabel;
			var ddl = new TagField(label: label, defaultValue: DEFAULT_TAG) {
				bindingPath = property.propertyPath,
			};

			ddl.AddToClassList(TagField.alignedFieldUssClassName);
			ddl.Bind(property.serializedObject);

			if (string.IsNullOrEmpty(property.stringValue))
			{
				property.stringValue = DEFAULT_TAG;
				property.serializedObject.ApplyModifiedProperties();
			}

			return ddl;
		}
	}
}
