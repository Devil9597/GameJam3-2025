using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Systems.Stats
{
	[CustomPropertyDrawer(typeof(StatFloat))]
	public class StatFloatDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new PropertyField(property.FindPropertyRelative("_baseValue"), property.displayName) {
				tooltip = property.tooltip,
			};
			field.AddToClassList(FloatField.alignedFieldUssClassName);
			field.Bind(property.serializedObject);
			return field;
		}
	}

	[CustomPropertyDrawer(typeof(StatInt))]
	public class StatIntDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new PropertyField(property.FindPropertyRelative("_baseValue"), property.displayName) {
				tooltip = property.tooltip,
			};
			field.AddToClassList(FloatField.alignedFieldUssClassName);
			field.Bind(property.serializedObject);
			return field;
		}
	}
}