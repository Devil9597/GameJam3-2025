using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.Extensions
{
	public static class ComponentExtensions
	{
		public static bool CompareTags(this GameObject obj, params string[] tags)
		{
			bool valid = false;
			for (int i = 0; i < tags.Length && !valid; i++)
			{
				valid = obj.CompareTag(tags[i]);
			}
			return valid;
		}

		public static bool CompareTags(this Component c, params string[] tags) => c.gameObject.CompareTags(tags);


		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			if (!gameObject.TryGetComponent(out T component))
			{
#if UNITY_EDITOR
				component = UnityEditor.Undo.AddComponent<T>(gameObject);
#else
			component = gameObject.AddComponent<T>();
#endif
			}
			return component;
		}
		public static T GetOrAddComponent<T>(this Component comp)
			where T : Component => comp.gameObject.GetOrAddComponent<T>();


		public static T OrNull<T>(this T obj) where T : Object => (bool)obj ? obj : null;
	}
}