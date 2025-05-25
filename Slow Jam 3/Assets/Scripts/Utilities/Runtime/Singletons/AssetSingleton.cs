using UnityEngine;

namespace Utilities.Singletons
{
	public abstract class AssetSingleton<T> : ScriptableObject
		where T : ScriptableObject
	{
		protected static T _instance;
		public static bool HasInstance => _instance != null;

		public static T Instance => GetOrCreateInstance();

		public static T GetOrCreateInstance()
		{
			if (!HasInstance)
			{
				_instance = SingletonHelpers.FindOrCreateScriptableObject<T>();
			}
			return _instance;
		}

		public static bool TryGetInstance(out T instance)
		{
			instance = _instance;
			return HasInstance;
		}

		public static void SetActiveInstance(T newInstance)
		{
			if (newInstance == null)
			{
				Debug.LogError($"Cannot assign the active instance to be null.");
			}
			else
			{
				_instance = newInstance;
			}
		}

		/// <summary>
		/// It is recommended to use the <see cref="Initialize"/> method for initialization logic instead. If you must override <see cref="Awake"/>, ensure that base.Awake() is still called to ensure the singleton is properly set up.
		/// </summary>
		protected virtual void Awake()
		{
			if (!Application.isPlaying)
				return;

			SetActiveInstance(this);
		}

		/// <summary>
		/// This method is invoked after this object has been set as the <see cref="Instance"/>.
		/// </summary>
		protected virtual void Initialize() { }

		public static implicit operator T(AssetSingleton<T> singleton)
		{
			if (singleton is T asset)
			{
				return asset;
			}
			else
			{
				throw SingletonHelpers.AssetConversionFailed<T>(nameof(AssetSingleton<T>));
			}
		}
	}
}
