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
			this.type = type;
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
						pattern = "/" + m.Name;
					}

					// Route it
					Route(pattern, attr.matchType ?? GuessMatchType(pattern), new ActionHandler(this, m).Invoke, attr.methods);
				}
			}
		}

		public Controller CreateControllerInstance()
		{
			return (Controller)Activator.CreateInstance(type);
		}

		static Manos.Routing.MatchType GuessMatchType(string pattern)
		{
			if (pattern.IndexOfAny(Manos.Routing.MatchOperationFactory.REGEX_CHARS) >= 0)
				return Manos.Routing.MatchType.Regex;
			if (pattern.IndexOfAny(Manos.Routing.MatchOperationFactory.SIMPLE_CHARS) >= 0)
				return Manos.Routing.MatchType.Simple;
			return Manos.Routing.MatchType.String;
		}



		public Type type;
		public MvcApp Application;
	}

}
