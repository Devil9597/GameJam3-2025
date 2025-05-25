using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Utilities.Timers
{
	/// <summary>
	/// A timer that counts down to zero over a given duration.
	/// </summary>
	public class CountdownTimer : ITimer
	{
		private float _timeElapsed;

		private CancellationTokenSource _tokenSource;
		private bool _disposedValue;

		public event ITimer.TimerEvent OnStart;
		public event ITimer.TimerEvent OnTick;
		public event ITimer.TimerEvent OnStop;

		public bool IsRunning { get; private set; }

		/// <summary>
		/// The amount of time in seconds which the timer will run for.
		/// </summary>
		public float Duration { get; private set; }

		/// <summary>
		/// The amount of time elapsed since the timer started.
		/// </summary>
		public float TimeElapsed { get => _timeElapsed; set => _timeElapsed = Mathf.Clamp(value, 0f, Duration); }

		/// <summary>
		/// Constructs an <see cref="CountdownTimer"/> with the given <paramref name="duration"/>.
		/// </summary>
		/// <param name="duration"></param>
		public CountdownTimer(float duration, bool repeat = false)
		{
			Duration = duration;

			if (repeat)
				OnStop += Repeat;
		}

		public void Start(bool sendCallback = true)
		{
			_tokenSource = new CancellationTokenSource();

			IsRunning = true;

			if (sendCallback)
				OnStart?.Invoke(this);

			RunAsync(_tokenSource.Token);
		}

		private void Repeat(ITimer timer) => Start();

		private async void RunAsync(CancellationToken cancellationToken)
		{
			Debug.Assert(IsRunning);
			float startTime = Time.time;
			TimeElapsed = 0;

			try
			{
				while (Time.time < startTime + Duration)
				{
					cancellationToken.ThrowIfCancellationRequested();
					
					await Awaitable.EndOfFrameAsync(cancellationToken);
					
					TimeElapsed = Time.time - startTime;
					OnTick?.Invoke(this);
				}

				Stop();
			}
			catch (OperationCanceledException)
			{
				IsRunning = false;
				Debug.Log($"{Duration}-second countdown canceled");
			}
		}

		public void Stop(bool sendCallback = true)
		{
			_tokenSource?.Cancel();
			_tokenSource?.Dispose();
			_tokenSource = null;

			if (sendCallback && IsRunning)
			{
				IsRunning = false;
				OnStop?.Invoke(this);
			}
			else
			{
				IsRunning = false;
			}
		}

		/// <summary>
		/// Resets <see cref="TimeElapsed"/> to zero.
		/// </summary>
		/// <remarks>
		/// See also: <seealso cref="Restart(bool)"/>
		/// </remarks>
		public void Reset()
		{
			TimeElapsed = 0.0f;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					OnStart = OnStart.Cleared();
					OnStop = OnStop.Cleared();
					OnTick = OnTick.Cleared();
				}

				Stop(sendCallback: false);
				_disposedValue = true;
			}
		}

		~CountdownTimer()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}