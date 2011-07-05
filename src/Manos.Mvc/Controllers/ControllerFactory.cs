using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Manos.Routing;

namespace Manos.Mvc
{
	public class ControllerFactory : ManosModule
	{
		public ControllerFactory(MvcApp app, Type type)
		{
			this.ControllerType = type;
			this.Application = app;

			// Route all actions
			foreach (var m in type.GetMethods())
			{
				foreach (HttpMethodAttribute attr in m.GetCustomAttributes(typeof(HttpMethodAttribute), true))
				{
					// Work out pattern
					string pattern = attr.pattern;
					if (pattern == null)
					{
						pattern = m.Name;
					}

					if (pattern.StartsWith("/"))
					{
						// Route it
						app.Route(pattern, attr.MatchType, new ActionHandler(this, m).InvokeMvcController, attr.methods);
					}
					else
					{
						// Route it
						this.Route("/" + pattern, attr.MatchType, new ActionHandler(this, m).InvokeMvcController, attr.methods);
					}
				}
			}
		}

		public Type ControllerType;
		public MvcApp Application;
	}

}
