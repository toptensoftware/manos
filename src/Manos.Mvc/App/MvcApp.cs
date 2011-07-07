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
			// Create services
			ControllerService = new ControllerService(this);
			ViewService = new ViewService(this);

			// Document root is the current directory
			DocumentRoot = System.IO.Directory.GetCurrentDirectory();

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

		// Route a static content folder
		public void RouteStaticContent(string route_prefix, string content_folder=null)
		{
			// If content folder not specified assume same as the route prefix
			if (content_folder == null)
				content_folder = route_prefix;

			// Setup routing to static content module
			Route(route_prefix, new StaticContentModule(route_prefix, MapPath(content_folder)));
		}

		public ISessionStateProvider SessionStateProvider
		{
			get;
			set;
		}

		public ControllerService ControllerService
		{
			get;
			private set;
		}

		public ViewService ViewService
		{
			get;
			private set;
		}

	}
}
