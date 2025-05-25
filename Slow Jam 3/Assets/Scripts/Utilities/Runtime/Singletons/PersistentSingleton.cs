using UnityEngine;

namespace Utilities.Singletons
{
	public abstract class PersistentSingleton<T> : MonoBehaviour
		where T : Component
	{
		protected static T _instance;
		public static bool HasInstance => _instance != null;

		public static T Instance => GetOrCreateInstance();

		public static T GetOrCreateInstance()
		{
				if (!HasInstance)
				{
					_instance = SingletonHelpers.FindOrCreateGameObject<T>();
				}
				return _instance;
		}

		public static bool TryGetInstance(out T instance)
		{
			instance = _instance;
			return HasInstance;
		}

		/// <summary>
		/// It is recommended to use the <see cref="Initialize"/> method for initialization logic instead. If you must override <see cref="Awake"/>, ensure that base.Awake() is still called to ensure the singleton is properly set up.
		/// </summary>
		protected virtual void Awake()
		{
			if (!Application.isPlaying)
				return;

			if (!HasInstance)
			{
				transform.SetParent(null);
				_instance = this;
				DontDestroyOnLoad(this.gameObject);
				Initialize();
			}
			else if (_instance != this)
			{
				Destroy(this.gameObject);
			}
		}

		/// <summary>
		/// This method is invoked after this object has been set as the <see cref="Instance"/>.
		/// </summary>
		protected abstract void Initialize();

		public static implicit operator T(PersistentSingleton<T> singleton)
		{
			if (singleton is T component || singleton != null && singleton.TryGetComponent(out component))
			{
				return component;
			}
			else
			{
				throw SingletonHelpers.ComponentConversionFailed<T>(nameof(PersistentSingleton<T>));
			}
		}
	}
}