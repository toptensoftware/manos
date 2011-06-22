using System;
using System.Threading;

namespace Manos.IO.Managed
{
	class TimerWatcher : Watcher, ITimerWatcher
	{
		private Action cb;
		private Timer timer;
		private TimeSpan after;
		private int Executing;

		public TimerWatcher (Context context, Action callback, TimeSpan after, TimeSpan repeat)
			: base (context)
		{
			if (callback == null)
				throw new ArgumentNullException ("callback");
			
			this.cb = callback;
			this.timer = new Timer (Invoke);
			this.after = after;
			this.Repeat = repeat;
			this.Executing = 0;
		}

		void Invoke (object state)
		{

			try
			{
				// Prevent re-entrancy
				if (System.Threading.Interlocked.Increment(ref Executing) > 1)
					return;

				if (IsRunning)
				{
					Context.Enqueue(cb);
					after = TimeSpan.Zero;
				}
			}
			finally
			{
				System.Threading.Interlocked.Decrement(ref Executing);
			}

		}

		public override void Start ()
		{
			base.Start ();
			timer.Change ((int) after.TotalMilliseconds,
				Repeat == TimeSpan.Zero ? -1 : (int) Repeat.TotalMilliseconds);
		}

		public override void Stop ()
		{
			timer.Change (Timeout.Infinite, Timeout.Infinite);
			base.Stop ();
		}

		protected override void Dispose (bool disposing)
		{
			Context.Remove (this);
		}

		public void Again ()
		{
			after = TimeSpan.Zero;
			Start ();
		}

		public TimeSpan Repeat {
			get;
			set;
		}
	}
}

