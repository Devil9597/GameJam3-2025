using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Extensions
{
	public delegate void CaseAction<in T>(T value);

	public readonly struct Case<T>
	{
		public readonly T @case;
		public readonly Predicate<T> condition;
		public readonly CaseAction<T> action;

		public Case(T @case, CaseAction<T> action)
		{
			this.@case = @case;
			this.condition = x => true;
			this.action = action;
		}

		public Case(T value, Predicate<T> condition, CaseAction<T> action)
		{
			this.@case = value;
			this.condition = condition;
			this.action = action;
		}

		public bool Compare(IEqualityComparer<T> comparer, T obj)
		{
			var result = @case is null || comparer.Equals(obj, @case);
			return result &= (condition ?? delegate { return true; })(obj);
		}

		public static implicit operator Case<T>((T, CaseAction<T>) obj) => new(obj.Item1, obj.Item2);
		public static implicit operator Case<T>((T, Predicate<T>, CaseAction<T>) obj) => new(obj.Item1, obj.Item2, obj.Item3);
	}

	public static class Generic
	{
		public static void Switch<T>(this T on, IEqualityComparer<T> comparer, CaseAction<T> defaultCase, params Case<T>[] cases)
		{
			for (int i = 0; i < cases.Length; i++)
			{
				if (cases[i].Compare(comparer, on))
				{
					(cases[i].action ?? delegate { })(cases[i].@case);
					return;
				}
			}
			(defaultCase ?? delegate { })(on);
		}

		public static void SwitchAll<T>(this T on, IEqualityComparer<T> comparer, CaseAction<T> defaultCase, params Case<T>[] cases)
		{
			for (int i = 0; i < cases.Length; i++)
			{
				if (cases[i].Compare(comparer, on))
				{
					(cases[i].action ?? delegate { })(cases[i].@case);
				}
			}
			(defaultCase ?? delegate { })(on);
		}

		public static T Chain<T>(this T obj, Action<T> action) { (action ?? delegate { })(obj); return obj; }
		public static T2 ChainTo<T1, T2>(this T1 obj, Func<T1, T2> func)
		{
			if (func is null) throw new ArgumentNullException(nameof(func));
			return func(obj);
		}
	}
}
