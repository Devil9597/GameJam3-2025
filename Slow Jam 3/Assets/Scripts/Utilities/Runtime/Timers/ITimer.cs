using System;

namespace Utilities.Timers
{
	/// <summary>
	/// Base interface for an asynchronous timer.
	/// </summary>
	public interface ITimer : IDisposable
	{
		/// <summary>
		/// Is <see langword="true"/> while the timer is running.
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// The amount of time passed since the timer started.
		/// </summary>
		float TimeElapsed { get; }

		/// <summary>
		/// Invoked when the timer starts.
		/// </summary>
		event TimerEvent OnStart;
		/// <summary>
		/// Invoked every time the timer updates.
		/// </summary>
		event TimerEvent OnTick;
		/// <summary>
		/// Invoked when the timer ends.
		/// </summary>
		event TimerEvent OnStop;

		public delegate void TimerEvent(ITimer timer);

		/// <summary>
		/// Starts the timer.
		/// </summary>
		/// <param name="sendCallback">If <see langword="true"/>, will invoke the <see cref="OnStart"/> event.</param>
		void Start(bool sendCallback = true);
		/// <summary>
		/// Stops the timer.
		/// </summary>
		/// <param name="sendCallback">If <see langword="true"/>, will invoke the <see cref="OnStop"/> event.</param>
		void Stop(bool sendCallback = true);
		/// <summary>
		/// Stops the timer before starting it again.
		/// </summary>
		/// <param name="sendCallback">If <see langword="true"/>, will invoke the <see cref="OnStop"/> and <see cref="OnStart"/> events.</param>
		public virtual void Restart(bool sendCallback = true)
		{
			Stop(sendCallback);
			Start(sendCallback);
		}
	}

	public static class DelegateExtensions
	{
		/// <summary>
		/// Clears all callback of the delegate and returns the result.
		/// </summary>
		/// <remarks>
		/// The method does not modify the original object. To apply the changes, you must assign the result of this method to the variable.
		/// </remarks>
		/// <typeparam name="TDelegate"></typeparam>
		/// <param name="delegate"></param>
		/// <returns></returns>
		public static TDelegate Cleared<TDelegate>(this TDelegate @delegate) where TDelegate : Delegate
		{
			return (TDelegate)Delegate.RemoveAll(@delegate, @delegate);
		}
	}
}