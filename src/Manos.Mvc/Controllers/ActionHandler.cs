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
		}

		public void Invoke(IManosContext ctx)
		{
			try
			{
				// Create the controller
				Controller controller = owner.CreateControllerInstance();

				// Create a context
				controller.Context = new ControllerContext()
				{
					Application = owner.Application,
					Controller = controller,
					CurrentAction = methodInfo,
					ManosContext = ctx,
				};

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
				var result = actionDelegate(controller, data);

				// Process the action result
				ActionResult action_result = result as ActionResult;
				if (action_result != null)
				{
					action_result.Process(controller.Context);
					return;
				}

				// Handle the result
				var string_result = result as string;
				if (string_result != null)
				{
					ctx.Response.End(string_result);
				}
				else
				{
					ctx.Response.End(result.ToString());
				}

				// Save modified session state
				controller.Context.OnPostRequest();
			}
			catch (Exception x)
			{
				ExceptionRenderer.RenderException(ctx, x, true);
			}
		}

		ControllerFactory owner;
		MethodInfo methodInfo;
		ActionDelegate actionDelegate;
	}
}
