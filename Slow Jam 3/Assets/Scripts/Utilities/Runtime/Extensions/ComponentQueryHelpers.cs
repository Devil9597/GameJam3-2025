using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HeirarchyPreference { Children, Parents }

namespace Utilities.Extensions
{
	public static class ComponentQueryHelpers
	{
		public static IEnumerable<Transform> EnumerateChildren(this Transform transform)
		{
			for (int i = 0; i < transform.childCount; i++)
				yield return transform.GetChild(i);
		}

		/// <summary>
		/// Begins a component query for this game object.
		/// </summary>
		/// <param name="gameObject"></param>
		/// <returns></returns>
		public static Q_Get Get(this GameObject gameObject) => new(gameObject);
		/// <summary>
		/// <inheritdoc cref="Get(GameObject)"/>
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public static Q_Get Get(this Component component) => new(component.gameObject);

		public readonly struct Q_Get
		{
			private readonly GameObject _object;

			public Q_Get(GameObject obj) => _object = obj;

			/// <summary>
			/// First component of type <typeparamref name="T"/> ...
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <returns></returns>
			public Q_Component<T> Component<T>() => new(_object);
			/// <summary>
			/// <inheritdoc cref="Component{T}()"/>
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="predicate"></param>
			/// <returns></returns>
			public Q_Component<T> Component<T>(Func<T, bool> predicate) => new(_object, predicate);

			public readonly struct Q_Component<T>
			{
				private readonly GameObject _object;
				private readonly Func<T, bool> _predicate;

				public Q_Component(GameObject obj, Func<T, bool> predicate = null) => (_object, _predicate) = (obj, predicate);

				#region GetComponent
				/// <summary>
				/// ... on this GameObject.
				/// </summary>
				/// <returns></returns>
				public T OnSelf() => _predicate switch {
					null => _object.GetComponent<T>(),
					_ => _object.GetComponents<T>().FirstOrDefault(_predicate)
				};
				/// <summary>
				/// ... in a parent of this GameObject.
				/// </summary>
				/// <param name="includeInactive"></param>
				/// <returns></returns>
				public T InParent(bool includeInactive = false) => _predicate switch {
					null => _object.GetComponentInParent<T>(includeInactive),
					_ => _object.GetComponentsInParent<T>(includeInactive).FirstOrDefault(_predicate),
				};
				/// <summary>
				/// ... in a child of this GameObject.
				/// </summary>
				/// <param name="includeInactive"></param>
				/// <returns></returns>
				public T InChildren(bool includeInactive = false) => _predicate switch {
					null => _object.GetComponentInChildren<T>(includeInactive),
					_ => _object.GetComponentsInChildren<T>(includeInactive).FirstOrDefault(_predicate)
				};
				/// <summary>
				/// ... anywhere in this GameObject's heirarchy. (parents, children, and self)
				/// </summary>
				/// <param name="includeInactive"></param>
				/// <param name="preference"></param>
				/// <returns></returns>
				public T InHeirarchy(bool includeInactive = false, HeirarchyPreference preference = HeirarchyPreference.Children)
				{
					TryGet step1 = InChildren, step2 = InParent;
					if (preference is HeirarchyPreference.Parents)
						(step1, step2) = (step2, step1);

					if (!step1(out T c, includeInactive))
						step2(out c, includeInactive);
					return c;
				}
				private delegate bool TryGet(out T c, bool includeInactive);
				#endregion

				#region TryGetComponent
				/// <summary>
				/// <inheritdoc cref="OnSelf()"/>
				/// </summary>
				/// <param name="component"></param>
				/// <returns><see langword="true"/> if the component exists.</returns>
				public bool OnSelf(out T component)
				{
					component = OnSelf();
					return component != null;
				}
				/// <summary>
				/// <inheritdoc cref="InParent(bool, bool)"/>
				/// </summary>
				/// <param name="component"></param>
				/// <param name="includeInactive"></param>
				/// <returns><see langword="true"/> if the component exists.</returns>
				public bool InParent(out T component, bool includeInactive = false)
				{
					component = InParent(includeInactive);
					return component != null;
				}
				/// <summary>
				/// <inheritdoc cref="InChildren(bool, bool)"/>
				/// </summary>
				/// <param name="component"></param>
				/// <param name="includeInactive"></param>
				/// <returns><see langword="true"/> if the component exists.</returns>
				public bool InChildren(out T component, bool includeInactive = false)
				{
					component = InChildren(includeInactive);
					return component != null;
				}
				/// <summary>
				/// <inheritdoc cref="InHeirarchy(bool, HeirarchyPreference)"/>
				/// </summary>
				/// <param name="component"></param>
				/// <param name="includeInactive"></param>
				/// <param name="preference"></param>
				/// <returns><see langword="true"/> if the component exists.</returns>
				public bool InHeirarchy(out T component, bool includeInactive = false, HeirarchyPreference preference = HeirarchyPreference.Children)
				{
					component = InHeirarchy(includeInactive, preference);
					return component != null;
				}
				#endregion
			}

			/// <summary>
			/// All components of type <typeparamref name="T"/> ...
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <returns></returns>
			public Q_Components<T> Components<T>() => new(_object);
			/// <summary>
			/// <inheritdoc cref="Components{T}()"/>
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="predicate"></param>
			/// <returns></returns>
			public Q_Components<T> Components<T>(Func<T, bool> predicate) => new(_object, predicate);

			public readonly struct Q_Components<T>
			{
				private readonly GameObject _object;
				private readonly Func<T, bool> _predicate;

				public Q_Components(GameObject obj, Func<T, bool> predicate = null) => (_object, _predicate) = (obj, predicate);

				#region GetComponents
				/// <summary>
				/// ... on this GameObject.
				/// </summary>
				/// <returns></returns>
				public T[] OnSelf() => _predicate switch {
					null => _object.GetComponents<T>(),
					_ => _object.GetComponents<T>().Where(_predicate).ToArray()
				};
				/// <summary>
				/// ... in a parent of this GameObject.
				/// </summary>
				/// <param name="includeInactive"></param>
				/// <returns></returns>
				public T[] InParent(bool includeInactive = false) => _predicate switch {
					null => _object.GetComponentsInParent<T>(includeInactive),
					_ => _object.GetComponentsInParent<T>(includeInactive).Where(_predicate).ToArray()
				};
				/// <summary>
				/// ... in a child of this GameObject.
				/// </summary>
				/// <param name="includeInactive"></param>
				/// <returns></returns>
				public T[] InChildren(bool includeInactive = false) => _predicate switch {
					null => _object.GetComponentsInChildren<T>(includeInactive),
					_ => _object.GetComponentsInChildren<T>(includeInactive).Where(_predicate).ToArray()
				};
				/// <summary>
				/// ... anywhere in this GameObject's heirarchy. (parents, children, and self)
				/// </summary>
				/// <param name="includeInactive"></param>
				/// <returns></returns>
				public T[] InHeirarchy(bool includeInactive = false)
				{
					IEnumerable<T> components = _object.GetComponentsInParent<T>(includeInactive)
						.Concat(_object.GetComponentsInChildren<T>(includeInactive))
						.Distinct();
					return _predicate switch {
						null => components.ToArray(),
						_ => components.Where(_predicate).ToArray()
					};
				}
				#endregion

				#region TryGetComponents
				/// <summary>
				/// <inheritdoc cref="OnSelf()"/>
				/// </summary>
				/// <param name="components"></param>
				/// <returns><see langword="true"/> if any <typeparamref name="T"/> component was found.</returns>
				public bool OnSelf(out T[] components)
				{
					components = _object.Get().Components(_predicate).OnSelf();
					return components is not null && components.Length > 0;
				}
				/// <summary>
				/// <inheritdoc cref="InParent(bool, bool)"/>
				/// </summary>
				/// <param name="components"></param>
				/// <param name="includeInactive"></param>
				/// <returns><see langword="true"/> if any <typeparamref name="T"/> component was found.</returns>
				public bool InParent(out T[] components, bool includeInactive = false)
				{
					components = _object.Get().Components(_predicate).InParent(includeInactive);
					return components is not null && components.Length > 0;
				}
				/// <summary>
				/// <inheritdoc cref="InChildren(bool, bool)"/>
				/// </summary>
				/// <param name="components"></param>
				/// <param name="includeInactive"></param>
				/// <returns><see langword="true"/> if any <typeparamref name="T"/> component was found.</returns>
				public bool InChildren(out T[] components, bool includeInactive = false)
				{
					components = _object.Get().Components(_predicate).InChildren(includeInactive);
					return components is not null && components.Length > 0;
				}
				/// <summary>
				/// <inheritdoc cref="InHeirarchy(bool)"/>
				/// </summary>
				/// <param name="components"></param>
				/// <param name="includeInactive"></param>
				/// <returns><see langword="true"/> if any <typeparamref name="T"/> component was found.</returns>
				public bool InHeirarchy(out T[] components, bool includeInactive = false)
				{
					components = _object.Get().Components(_predicate).InHeirarchy(includeInactive);
					return components is not null && components.Length > 0;
				}
				#endregion
			}
		}
	}
}
