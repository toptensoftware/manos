using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public struct PendingOperations
	{
		public PendingOperations(AsyncResult owner, int initalValue)
		{
			this.owner = owner;
			this.value = initalValue;
		}

		AsyncResult owner;
		int value;

		public void Increment(int delta = 1)
		{
			if (System.Threading.Interlocked.Add(ref value, delta) == 0)
			{
				owner.Complete();
			}
		}

		public void Decrement(int delta = 1)
		{
			Increment(-delta);
		}

		public int Value
		{
			get
			{
				return value;
			}
		}

	
	}

	public class AsyncResult : ActionResult
	{
		public AsyncResult(int initialPendingOperations=0)
		{
			PendingOperations = new PendingOperations(this, initialPendingOperations);
		}

		public PendingOperations PendingOperations
		{
			get;
			private set;
		}

		internal void Complete()
		{
			if (pendingContext != null && onComplete!=null)
			{
				try
				{
					pendingContext.ProcessResult(onComplete());
				}
				catch (Exception x)
				{
					ExceptionRenderer.RenderException(pendingContext.ManosContext, x, true);
				}
			}
		}

		public Func<object> onComplete;
		
		ControllerContext pendingContext;

		#region IActionResult Members

		public void Process(ControllerContext ctx)
		{
			// Just store the pending context, we'll complete it later
			pendingContext = ctx;

			// Already finished?
			if (PendingOperations.Value == 0)
			{
				Complete();
			}
		}

		#endregion
	}
}
