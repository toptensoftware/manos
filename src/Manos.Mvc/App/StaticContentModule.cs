using System;
using System.IO;

using Manos;
using Manos.Routing;

//
//  This the default StaticContentModule that comes with all Manos apps
//  if you do not wish to serve any static content with Manos you can
//  remove its route handler from <YourApp>.cs's constructor and delete
//  this file.
//
//  All Content placed on the Content/ folder should be handled by this
//  module.
//

namespace Manos.Mvc
{
	public class StaticContentModule : ManosModule 
	{
		private string content_folder;
		private string route_prefix;

		public StaticContentModule(string route_prefix, string content_folder)
		{
			this.route_prefix = route_prefix;
			this.content_folder = content_folder;

			Get (".*", MatchType.Regex, Content);
		}

		public void Content (IManosContext ctx)
		{
			string path = ctx.Request.Path;

			// Double check path start with route-prefix
			if (path.StartsWith(route_prefix, StringComparison.InvariantCultureIgnoreCase) && path.IndexOf("..") < 0)
			{
				// Strip off the route prefix and leading slash
				path = path.Substring(route_prefix.Length);
				if (path.StartsWith("/"))
					path = path.Substring(1);

				// Locate the file
				path = Path.Combine(content_folder, path);

				// Check it exists
				if (File.Exists(path))
				{
					// Send it
					ctx.Response.Headers.SetNormalizedHeader("Content-Type", ManosMimeTypes.GetMimeType(path));
					ctx.Response.SendFile(path);
					ctx.Response.End();
					return;
				}
			}

			ctx.Response.StatusCode = 404;
			ctx.Response.End();
		}

	}
}

