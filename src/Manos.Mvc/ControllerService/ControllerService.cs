using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Manos.Mvc
{
	public class ControllerService
	{
		public ControllerService(MvcApp app)
		{
			this.Application = app;
		}

		public MvcApp Application
		{
			get;
			private set;
		}

		// Enumerate all types in the calling assembly and register all Controllers
		public void RegisterAllControllers(bool UseThreadPool = true)
		{
			RegisterAllControllers(Assembly.GetCallingAssembly(), UseThreadPool);
		}

		public static string CleanControllerName(string str)
		{
			if (str.EndsWith("Controller"))
				return str.Substring(0, str.Length - 10);
			else
				return str;
		}


		// Register all controllers for a specific assembly
		public void RegisterAllControllers(Assembly assembly, bool UseThreadPool = true)
		{
			// Find all controllers
			foreach (var type in from t in assembly.GetTypes() where !t.IsAbstract && typeof(Controller).IsAssignableFrom(t) select t)
			{
				RegisterController(type, UseThreadPool);
			}
		}

		// Register a single controller type
		public void RegisterController(Type type, bool UseThreadPool = true)
		{
			// Create a factory for it
			var rm = new ControllerFactory(this, type, UseThreadPool);

			var cleanName = CleanControllerName(type.Name);

			// Map all it's routes
			bool any_routes = false;
			foreach (HttpControllerAttribute attr in type.GetCustomAttributes(typeof(HttpControllerAttribute), false))
			{
				if (attr.pattern != null)
					Application.Route(attr.pattern, rm);
				else
					Application.Route("/" + cleanName, rm);

				any_routes = true;
			}

			// Map default route if not specified by at least one attribute
			if (!any_routes)
			{
				Application.Route("/" + cleanName, rm);
			}

			// Register fully qualified name eg: "MyProject.Controllers.HomeController"
			ControllerTypes.Add(type.FullName, type);

			// Register normal name eg: "HomeController"
			ControllerTypes.Add(type.Name, type);

			// Register shortened name eg: "Home"
			if (cleanName != type.Name)
				ControllerTypes.Add(cleanName, type);
		}

		// Replace this to install custom controller instantiation (IoC)
		public Func<ControllerContext, Type, Controller> CreateControllerInstance =
			(cc, t) => (Controller)Activator.CreateInstance(t);

		private Dictionary<string, Type> ControllerTypes = new Dictionary<string, Type>();

		public Type GetControllerType(string name)
		{
			// Look up controller name
			Type t;
			if (ControllerTypes.TryGetValue(name, out t))
				return t;
			return null;
		}
	}
}
