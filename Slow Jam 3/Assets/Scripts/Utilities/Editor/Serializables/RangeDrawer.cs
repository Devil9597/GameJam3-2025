using UnityEditor;
using UnityEngine.UIElements;

namespace Utilities.Serializables.Editor
{
	[CustomPropertyDrawer(typeof(RangeInt))]
	internal class RangeIntDrawer : PropertyDrawer
	{
		const int _LABEL_FLEX_BASIS = 30;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new Vector2IntField(property.displayName) {
				value = (RangeInt)property.boxedValue
			};

			field.RegisterValueChangedCallback(evt => {
				property.boxedValue = (RangeInt)evt.newValue;
				property.serializedObject.ApplyModifiedProperties();
			});
			field.AddToClassList(Vector2IntField.alignedFieldUssClassName);

			var fields = field.Query<IntegerField>().ToList();

			for (int i = 0; i < fields.Count; i++)
			{
				fields[i].labelElement.style.flexBasis = _LABEL_FLEX_BASIS;
				fields[i].label = i switch {
					0 => "Min",
					_ => "Max"
				};
			}

			return field;
		}
	}

	[CustomPropertyDrawer(typeof(RangeFloat))]
	internal class RangeFloatDrawer : PropertyDrawer
	{
		const int _LABEL_FLAX_BASIS = 30;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new Vector2Field(property.displayName) {
				value = (RangeFloat)property.boxedValue
			};

			field.RegisterValueChangedCallback(evt => {
				property.boxedValue = (RangeFloat)evt.newValue;
				property.serializedObject.ApplyModifiedProperties();
			});
			field.AddToClassList(Vector2Field.alignedFieldUssClassName);

			var fields = field.Query<FloatField>().ToList();
			for (int i = 0; i < fields.Count; i++)
			{
				fields[i].labelElement.style.flexBasis = _LABEL_FLAX_BASIS;
				fields[i].label = i switch {
					0 => "Min",
					_ => "Max"
				};
			}

			return field;
		}
	}
}