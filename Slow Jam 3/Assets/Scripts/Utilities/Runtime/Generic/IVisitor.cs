using System;
using UnityEngine;

namespace Utilities
{
	public interface IVisitor<in T> where T : IVisitable<T>
	{
		/// <summary>
		/// Visits the <paramref name="obj"/>
		/// </summary>
		/// <param name="obj"></param>
		void Visit(T obj);
	}

	public interface IVisitable<T> where T : IVisitable<T>
	{
		/// <summary>
		/// Returns <see langword="true"/> if the <paramref name="visitor"/> is allowed to visit.
		/// </summary>
		/// <param name="visitor"></param>
		/// <returns></returns>
		bool Accept(IVisitor<T> visitor);

		/// <summary>
		/// Returns <see langword="true"/> and performs the <paramref name="visitAction"/> if the <paramref name="visitor"/> is allowed to visit.
		/// </summary>
		/// <param name="visitor"></param>
		/// <param name="visitAction"></param>
		/// <returns></returns>
		virtual bool Accept(IVisitor<T> visitor, Action<T> visitAction)
		{
			bool accepted = this.Accept(visitor);
			if (accepted)
			{
				Debug.Assert(this is T);
				visitAction((T)this);
			}
			return accepted;
		}
	}
}
