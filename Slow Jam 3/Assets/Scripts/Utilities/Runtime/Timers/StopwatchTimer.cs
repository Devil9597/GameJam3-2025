using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Utilities.Timers
{
	/// <summary>
	/// A timer that counts up until stopped.
	/// </summary>
	public class StopwatchTimer : ITimer
	{
		private CancellationTokenSource _tokenSource;
		private bool _disposedValue;

		public event ITimer.TimerEvent OnStart;
		public event ITimer.TimerEvent OnTick;
		public event ITimer.TimerEvent OnStop;

		public float TimeElapsed { get; private set; }
		public bool IsRunning { get; private set; }

		public void Start(bool sendCallback = true)
		{
			_tokenSource = new CancellationTokenSource();
			TimeElapsed = 0f;

			IsRunning = true;

			if (sendCallback)
				OnStart?.Invoke(this);

			RunAsync(_tokenSource.Token);
		}

		private async void RunAsync(CancellationToken cancellationToken)
		{
			float startTime = Time.time;

			try
			{
				while (IsRunning)
				{
					cancellationToken.ThrowIfCancellationRequested();
					await Awaitable.EndOfFrameAsync(cancellationToken);
					TimeElapsed = Time.time - startTime;
					OnTick?.Invoke(this);
				}
			}
			catch (OperationCanceledException)
			{
				IsRunning = false;
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

		~StopwatchTimer()
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