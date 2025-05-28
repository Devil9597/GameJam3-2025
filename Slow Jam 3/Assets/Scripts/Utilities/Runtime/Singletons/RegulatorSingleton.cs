using UnityEngine;

namespace Utilities.Singletons
{
	public abstract class RegulatorSingleton<T> : MonoBehaviour where T : Component
	{
		private static int _currentId = 0;
		private int _id;

		protected static T _instance;
		public static bool HasInstance => _instance != null;

		public static T Instance => GetOrCreateInstance();
		public static int Id { get; private set; }

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

		public static bool TrySetInstance(RegulatorSingleton<T> newInstance)
		{
			if (Id < newInstance._id)
			{
				SetInstance(newInstance, true);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// It is recommended to use the <see cref="Initialize"/> method for initialization logic instead. If you must override <see cref="Awake"/>, ensure that base.Awake() is still called to ensure the singleton is properly set up.
		/// </summary>
		protected virtual void Awake()
		{
			if (!Application.isPlaying)
				return;

			_id = ++_currentId;

			TrySetInstance(this);
		}

		private static void SetInstance(RegulatorSingleton<T> newInstance, bool destroyPrevious = true)
		{
			if (HasInstance && destroyPrevious)
			{
				Destroy(_instance.gameObject);
			}

			Id = newInstance._id;
			_instance = newInstance;
			newInstance.Initialize();
		}

		/// <summary>
		/// This method is invoked after this object has been set as the <see cref="Instance"/>.
		/// </summary>
		protected virtual void Initialize() { }

		public static implicit operator T(RegulatorSingleton<T> singleton)
		{
			if (singleton is T component || singleton != null && singleton.TryGetComponent(out component))
			{
				return component;
			}
			else
			{
				throw SingletonHelpers.ComponentConversionFailed<T>(nameof(RegulatorSingleton<T>));
			}
		}
	}
}