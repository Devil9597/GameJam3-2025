using UnityEngine;

namespace Utilities.Singletons
{
	internal static class SingletonHelpers
	{
		public static T FindOrCreateGameObject<T>() where T : Component
		{
			T instance = Object.FindAnyObjectByType<T>();
			if (instance == null)
			{
				instance = new GameObject(name: $"{typeof(T).Name} (Auto-Generated)").AddComponent<T>();
				LogObjectCreation<T>();
			}
			return instance;
		}

		public static T FindOrCreateScriptableObject<T>() where T : ScriptableObject
		{
			T instance = Object.FindAnyObjectByType<T>();
			if (instance == null)
			{
				instance = ScriptableObject.CreateInstance<T>();
				instance.name = $"{typeof(T).Name} (Auto-Generated)";
				LogObjectCreation<T>();
			}
			return instance;
		}

		public static void LogObjectCreation<T>()
		{
			Debug.Log($"Created a supplementary {typeof(T)} instance as the instance property was accessed but no instances existed.");
		}

		public static System.Exception ComponentConversionFailed<T>(string singletonTypeName)
		{
			return new System.InvalidCastException($"Failed to convert between {singletonTypeName} and {typeof(T)}. {typeof(T)} should be a {nameof(Component)} that either inherits from {singletonTypeName} or is attached to the same object as the singleton.");
		}

		public static System.Exception AssetConversionFailed<T>(string singletonTypeName)
		{
			return new System.InvalidCastException($"Failed to convert between {singletonTypeName} and {typeof(T)}. {typeof(T)} must be a {nameof(ScriptableObject)} that inherits from {singletonTypeName}.");
		}
	}
}