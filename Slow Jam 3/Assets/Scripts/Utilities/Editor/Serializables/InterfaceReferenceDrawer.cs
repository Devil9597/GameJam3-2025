using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.Serializables.Editor
{
	[CustomPropertyDrawer(typeof(InterfaceReference<>))]
	[CustomPropertyDrawer(typeof(InterfaceReference<,>))]
	public class InterfaceReferenceDrawer : PropertyDrawer
	{
		const string UNDERLYING_VALUE_FIELD_NAME = "_underlyingValue";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var underlyingProperty = property.FindPropertyRelative(UNDERLYING_VALUE_FIELD_NAME);
			InterfaceArgs args = GetArguments(fieldInfo: fieldInfo);

			EditorGUI.BeginProperty(position, label, property);
			var assignedObject = EditorGUI.ObjectField(position, label, underlyingProperty.objectReferenceValue, typeof(Object), true);

			if (assignedObject != null)
			{
				if (assignedObject is GameObject gameObject)
				{
					ValidateAndAssign(underlyingProperty, gameObject.GetComponent(args.interfaceType), gameObject.name, args.interfaceType.Name);
				}
				else
				{
					ValidateAndAssign(underlyingProperty, assignedObject, args.interfaceType.Name);
				}
			}
			else
			{
				underlyingProperty.objectReferenceValue = null;
			}

			EditorGUI.EndProperty();
		}

		private static InterfaceArgs GetArguments(FieldInfo fieldInfo)
		{
			Type objectType = null, interfaceType = null;
			Type fieldType = fieldInfo.FieldType;

			static bool tryGetTypesFromInterfaceReference(Type type, out Type objType, out Type intfType)
			{
				objType = intfType = null;

				if (type?.IsGenericType != true)
					return false;

				var genericType = type.GetGenericTypeDefinition();

				if (genericType == typeof(InterfaceReference<>))
					type = type.BaseType;

				if (type?.GetGenericTypeDefinition() == typeof(InterfaceReference<,>))
				{
					var types = type.GetGenericArguments();
					(intfType, objType) = (types[0], types[1]);
					return true;
				}

				return false;
			}
			static void getTypesFromList(Type type, out Type objType, out Type intfType)
			{
				objType = intfType = null;
				var listInterface = type.GetInterfaces()
					.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));

				if (listInterface != null)
				{
					var elementType = listInterface.GetGenericArguments()[0];
					tryGetTypesFromInterfaceReference(elementType, out objType, out intfType);
				}
			}

			if (!tryGetTypesFromInterfaceReference(fieldType, out objectType, out interfaceType))
			{
				getTypesFromList(fieldType, out objectType, out interfaceType);
			}

			return new InterfaceArgs(objectType, interfaceType);
		}

		static void ValidateAndAssign(SerializedProperty property, Object targetObject, string componentTypeOrName, string interfaceName = null)
		{
			if (targetObject != null)
			{
				property.objectReferenceValue = targetObject;
			}
			else
			{
				var message = interfaceName != null
					? $"GameObject '{componentTypeOrName}'"
					: $"assigned object";
				Debug.LogWarning($"The {message} does not have a component that implements '{componentTypeOrName}'.");
				property.objectReferenceValue = null;
			}
		}
	}

	public readonly struct InterfaceArgs
	{
		public readonly Type objectType, interfaceType;

		public InterfaceArgs(Type objectType, Type interfaceType)
		{
			Debug.Assert(typeof(Object).IsAssignableFrom(objectType), $"{nameof(objectType)} must be of type {typeof(Object)}.");
			Debug.Assert(interfaceType.IsInterface, $"{nameof(interfaceType)} must be an interface.");

			this.objectType = objectType;
			this.interfaceType = interfaceType;
		}

		public readonly void Deconstruct(out Type objectType, out Type interfaceType)
		{
			(objectType, interfaceType) = (this.objectType, this.interfaceType);
		}
	}

	public static class InterfaceReferenceUtil
	{
		static GUIStyle _labelStyle;

		public static void OnGUI(Rect position, SerializedProperty property, GUIContent label, InterfaceArgs args)
		{
			InitializeStyle();
			var controlID = GUIUtility.GetControlID(FocusType.Passive) - 1;
			var isHovering = position.Contains(Event.current.mousePosition);
			var displayString = property.objectReferenceValue == null || isHovering ? $"({args.interfaceType.Name})" : "*";
			DrawInterfaceNameLabel(position, displayString, controlID);
		}

		static void DrawInterfaceNameLabel(Rect position, string displayString, int controlID)
		{
			if (Event.current.type is EventType.Repaint)
			{
				const int width = 3;
				const int vIndent = 1;

				var content = EditorGUIUtility.TrTextContent(displayString);
				var size = _labelStyle.CalcSize(content);
				var labelPos = position;

				labelPos.width = size.x + width;
				labelPos.x += position.width - labelPos.width - 18;
				labelPos.height -= vIndent * 2;
				labelPos.y += vIndent;
				_labelStyle.Draw(labelPos, EditorGUIUtility.TrTextContent(displayString), controlID, DragAndDrop.activeControlID == controlID, false);
			}
		}

		static void InitializeStyle()
		{
			if (_labelStyle != null)
				return;

			var style = new GUIStyle(EditorStyles.label) {
				font = EditorStyles.objectField.font,
				fontSize = EditorStyles.objectField.fontSize,
				fontStyle = EditorStyles.objectField.fontStyle,
				alignment = TextAnchor.MiddleRight,
				padding = new RectOffset(0, 2, 0, 0)
			};
			_labelStyle = style;
		}
	}
}
