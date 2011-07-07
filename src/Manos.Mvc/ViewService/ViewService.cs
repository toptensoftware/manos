using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class ViewService
	{
		public ViewService(MvcApp app)
		{
			Application = app;
		
			// Register standard view paths
			RegisterViewPath("/Views/{controller}/{view}.cshtml");
			RegisterViewPath("/Views/Shared/{view}.cshtml");

			// Built-in razor view engine
			RegisterViewEngine(new RazorViewEngine(this));
		}

		public MvcApp Application
		{
			get;
			private set;
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
				var resolved = Application.MapPath(p.Replace("{view}", view).Replace("{controller}", controller));
				if (System.IO.File.Exists(resolved))
					return resolved;
			}

			return null;
		}


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
			string viewfile = FindViewTemplate(viewname, ControllerService.CleanControllerName(controller));
			if (viewfile == null)
				throw new InvalidOperationException(string.Format("Can't find view `{0}` for controller `{1}`", viewname, controller));

			// Create the view
			return LoadViewTemplate(viewfile);
		}

		Dictionary<string, IViewTemplate> m_Views = new Dictionary<string, IViewTemplate>();
		List<IViewEngine> m_ViewEngines = new List<IViewEngine>();
		List<string> m_ViewPaths = new List<string>();
	}
}
