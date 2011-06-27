using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Manos.Mvc
{
	public class MvcApp : ManosApp
	{
		public MvcApp()
		{
			// Document root is the current directory
			DocumentRoot = System.IO.Directory.GetCurrentDirectory();

			// Register standard view paths
			RegisterViewPath("/Views/Shared/{view}.cshtml");
			RegisterViewPath("/Views/{controller}/{view}.cshtml");

			// Built-in razor view engine
			RegisterViewEngine(new RazorViewEngine(this));

			// Default session state provider
			SessionStateProvider = new InMemorySessionStateProvider();

			// Allocate server key
			ServerKey = Guid.NewGuid().ToString();
		}

		public string ServerKey
		{
			get;
			set;
		}

		// Reset the view path
		public void ResetViewPath()
		{
			m_ViewPaths.Clear();
		}

		// Register a path on which to search for view files
		public void RegisterViewPath(string path)
		{
			m_ViewPaths.Add(path);
		}

		public void ResetViewEngines()
		{
			m_ViewEngines.Clear();
		}

		public void RegisterViewEngine(IViewEngine e)
		{
			m_ViewEngines.Add(e);
		}

		// Find a view
		public string FindViewTemplate(string view, string controller)
		{
			foreach (var p in m_ViewPaths)
			{
				var resolved = MapPath(p.Replace("{view}", view).Replace("{controller}", controller));
				if (System.IO.File.Exists(resolved))
					return resolved;
			}

			return null;
		}

		Dictionary<string, IViewTemplate> m_Views = new Dictionary<string, IViewTemplate>();
		List<IViewEngine> m_ViewEngines = new List<IViewEngine>();

		public IViewTemplate LoadViewTemplate(string viewfile)
		{
			// Check cache for an existing view factory
			lock (m_Views)
			{
				// Check if we've already got the factory
				IViewTemplate view;
				if (m_Views.TryGetValue(viewfile, out view))
					return view;

				// Find a view engine
				foreach (var e in m_ViewEngines)
				{
					view = e.CreateView(viewfile);
					if (view != null)
					{
						m_Views.Add(viewfile, view);
						return view;
					}
				}

				// No view engine knows how to handle this view
				return null;
			}
		}

		// Load a view engine
		public IViewTemplate LoadViewTemplate(string viewname, string controller)
		{
			string viewfile = FindViewTemplate(viewname, CleanControllerName(controller));
			if (viewfile==null)
				throw new InvalidOperationException(string.Format("Can't find view `{0}` for controller `{1}`", viewname, controller));

			// Create the view
			return LoadViewTemplate(viewfile);
		}

		// Map a path to the document root
		public string MapPath(string path)
		{
			if (!path.StartsWith("/"))
				throw new ArgumentException("Mapped paths must begin with a slash");

			path = System.IO.Path.Combine(DocumentRoot, path.Substring(1));

			path = path.Replace('/', System.IO.Path.DirectorySeparatorChar);
			return path;
		}

		// The document root folder
		public string DocumentRoot
		{
			get;
			set;
		}

		List<string> m_ViewPaths = new List<string>();

		// Route a static content folder
		public void RouteStaticContent(string route_prefix, string content_folder=null)
		{
			// If content folder not specified assume same as the route prefix
			if (content_folder == null)
				content_folder = route_prefix;

			// Setup routing to static content module
			Route(route_prefix, new StaticContentModule(route_prefix, MapPath(content_folder)));
		}

		// Enumerate all types in the calling assembly and register all Controllers
		public void RegisterAllControllers()
		{
			RegisterAllControllers(Assembly.GetCallingAssembly());
		}

		static string CleanControllerName(string str)
		{
			if (str.EndsWith("Controller"))
				return str.Substring(0, str.Length - 10);
			else
				return str;
		}


		// Register all controllers for a specific assembly
		public void RegisterAllControllers(Assembly assembly)
		{
			// Find all controllers
			foreach (var type in from t in assembly.GetTypes() where !t.IsAbstract && typeof(Controller).IsAssignableFrom(t) select t)
			{
				// Create a factory for it
				var rm = new ControllerFactory(this, type);

				// Map all it's routes
				bool any_routes = false;
				foreach (HttpControllerAttribute attr in type.GetCustomAttributes(typeof(HttpControllerAttribute), false))
				{
					if (attr.pattern != null)
						Route(attr.pattern, rm);
					else
						Route("/" + CleanControllerName(type.Name), rm);

					any_routes = true;
				}

				// Map default route if not specified by at least one attribute
				if (!any_routes)
				{
					Route("/" + CleanControllerName(type.Name), rm);
				}
			}
		}

		public ISessionStateProvider SessionStateProvider
		{
			get;
			set;
		}
	}
}
