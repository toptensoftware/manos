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
		public ControllerFactory(ControllerService Service, Type type, bool UseThreadPool)
		{
			this.ControllerType = type;
			this.Service = Service;

			// Work out whether to use thread pool
			var utp = (UseThreadPoolAttribute)type.GetCustomAttributes(typeof(UseThreadPoolAttribute), false).FirstOrDefault();
			if (utp != null)
				this.UseThreadPool = utp.UseThreadPool;
			else
				this.UseThreadPool = UseThreadPool;

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
						Service.Application.Route(pattern, attr.MatchType, new ActionHandler(this, m).InvokeMvcController, attr.methods);
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
		public ControllerService Service;
		public bool UseThreadPool;
	}

}
