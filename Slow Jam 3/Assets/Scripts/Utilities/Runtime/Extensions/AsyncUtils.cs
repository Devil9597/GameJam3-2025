using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Utilities.Extensions
{
	public static class AsyncUtils
	{
		public static async Task WaitForAwake(this MonoBehaviour behaviour)
		{
			if (behaviour && !behaviour.didAwake)
				await WaitUntil(() => behaviour.didAwake, behaviour.destroyCancellationToken);
		}

		public static async Task WaitForStart(this MonoBehaviour behaviour)
		{
			if (behaviour && !behaviour.didStart)
				await WaitUntil(() => behaviour.didStart, behaviour.destroyCancellationToken);
		}

		public static async Task WaitUntil(this MonoBehaviour behaviour, Func<bool> condition)
		{
			if (condition is null)
				throw new ArgumentNullException(nameof(condition));

			if (behaviour && !condition())
				await WaitUntil(condition, behaviour.destroyCancellationToken);
		}

		public static async Task WaitUntil(Func<bool> condition, CancellationToken cancellationToken = default)
		{
			if (condition is null)
				throw new ArgumentNullException(nameof(condition));

			try
			{
				while (!condition())
				{
					cancellationToken.ThrowIfCancellationRequested();
					await Awaitable.NextFrameAsync(cancellationToken);
				}
			}
			catch (OperationCanceledException)
			{
				Debug.Log($"{nameof(WaitUntil)} operation cancelled");
			}
		}

		public static async Task WaitWhile(this MonoBehaviour behaviour, Func<bool> condition)
		{
			if (condition is null)
				throw new ArgumentNullException(nameof(condition));

			if (behaviour && condition())
				await WaitWhile(condition, behaviour.destroyCancellationToken);
		}

		public static async Task WaitWhile(Func<bool> condition, CancellationToken cancellationToken = default)
		{
			if (condition is null)
				throw new ArgumentNullException(nameof(condition));

			try
			{
				while (condition())
				{
					cancellationToken.ThrowIfCancellationRequested();
					await Awaitable.NextFrameAsync(cancellationToken);
				}
			}
			catch (OperationCanceledException)
			{
				Debug.Log($"{nameof(WaitWhile)} operation cancelled");
			}
		}
	}
}