using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.Serializables.Editor
{
	[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
	public class RequireInterfaceDrawer : PropertyDrawer
	{
		RequireInterfaceAttribute Attribute => attribute as RequireInterfaceAttribute;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Type iType = Attribute.interfaceType;
			EditorGUI.BeginProperty(position, label, property);

			if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
			{
				DrawArrayField(position, property, label, iType);
			}
			else
			{
				DrawObjectField(position, property, label, iType);
			}

			EditorGUI.EndProperty();
			var args = new InterfaceArgs(GetTypeOrElement(fieldInfo.FieldType), iType);
			InterfaceReferenceUtil.OnGUI(position, property, label, args);
		}

		void DrawArrayField(Rect position, SerializedProperty property, GUIContent label, Type interfaceType)
		{
			property.arraySize = EditorGUI.IntField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
				label.text + " Size", property.arraySize);

			float yOffset = EditorGUIUtility.singleLineHeight;
			for (int i = 0; i < property.arraySize; i++)
			{
				var element = property.GetArrayElementAtIndex(i);
				var elementRect = new Rect(position.x, position.y + yOffset, position.width, EditorGUIUtility.singleLineHeight);
				DrawObjectField(elementRect, element, new GUIContent($"Element {i}"), interfaceType);
				yOffset += EditorGUIUtility.singleLineHeight;
			}
		}

		void DrawObjectField(Rect position, SerializedProperty property, GUIContent label, Type interfaceType)
		{
			var oldRef = property.objectReferenceValue;
			var newRef = EditorGUI.ObjectField(position, label, oldRef, typeof(Object), true);

			if (newRef != null && newRef != oldRef)
			{
				ValiadteAndAssign(property, newRef, interfaceType);
			}
			else if (newRef == null)
			{
				property.objectReferenceValue = null;
			}
		}

		void ValiadteAndAssign(SerializedProperty property, Object newReference, Type interfaceType)
		{
			if (newReference is GameObject gameObject)
			{
				var component = gameObject.GetComponent(interfaceType);
				if (component != null)
				{
					property.objectReferenceValue = component;
					return;
				}
			}
			else if (interfaceType.IsAssignableFrom(newReference.GetType()))
			{
				property.objectReferenceValue = newReference;
				return;
			}

			Debug.LogWarning($"The assigned object does not implement '{interfaceType.Name}'.");
			property.objectReferenceValue = null;
		}

		Type GetTypeOrElement(Type type)
		{
			if (type.IsArray)
				return type.GetElementType();
			if (type.IsGenericType)
				return type.GetGenericArguments()[0];
			else
				return type;
		}
	}
}