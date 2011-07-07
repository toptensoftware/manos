using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Manos.Routing;

namespace Manos.Mvc
{
	public class ActionHandler
	{
		public ActionHandler(ControllerFactory owner, MethodInfo methodInfo)
		{
			this.owner = owner;
			this.methodInfo = methodInfo;
			this.actionDelegate = ActionDelegateFactory.Create(methodInfo);
			this.useThreadPool = true;

			var utp = (UseThreadPoolAttribute)methodInfo.GetCustomAttributes(typeof(UseThreadPoolAttribute), false).FirstOrDefault();
			if (utp != null)
				this.useThreadPool = utp.UseThreadPool;
			else
				this.useThreadPool = owner.UseThreadPool;
		}

		public void InvokeMvcController(IManosContext ctx)
		{
			if (useThreadPool)
			{
				// Invoke the action on the thread pool
				System.Threading.ThreadPool.QueueUserWorkItem(o=>InvokeControllerInternal((IManosContext)ctx), ctx);
			}
			else
			{
				// Invoke the controller on the event loop thread
				InvokeControllerInternal(ctx);
			}
		}


		void InvokeControllerInternal(IManosContext ctx)
		{
			try
			{
				// Create a context
				var Context = new ControllerContext()
				{
					Application = owner.Service.Application,
					CurrentAction = methodInfo,
					ManosContext = ctx,
				};

				// Create the controller
				Context.Controller = owner.Service.CreateControllerInstance(Context, owner.ControllerType);
				Context.Controller.Context = Context;

				// Get parameters from the incoming data
				object[] data;
				var pi = methodInfo.GetParameters();
				if (pi.Length > 0)
				{
					ParameterizedActionTarget.TryGetDataForParamList(methodInfo.GetParameters(), null, ctx, out data);
				}
				else
				{
					data = null;
				}

				// Invoke the controller
				var result = actionDelegate(Context.Controller, data);

				// Process the result
				Context.ProcessResult(result);
			}
			catch (Exception x)
			{
				ExceptionRenderer.RenderException(ctx, x, true);
			}
		}

		ControllerFactory owner;
		MethodInfo methodInfo;
		ActionDelegate actionDelegate;
		bool useThreadPool;
	}
}
