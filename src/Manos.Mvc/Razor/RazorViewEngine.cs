using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Web.Razor.Generator;

namespace Manos.Mvc
{
	public class RazorViewEngine : IViewEngine
	{
		#region IViewEngine Members

		public RazorViewEngine(ViewService service)
		{
			this.Service = service;
		}

		ViewService Service;

		public IViewTemplate CreateView(string viewfile)
		{
			// Is it a razor view file?
			if (!viewfile.EndsWith(".cshtml", StringComparison.InvariantCultureIgnoreCase))
				return null;

			var compiler = new RazorViewCompiler();
			compiler.BaseClass = typeof(View);
			compiler.ViewFile = viewfile;

			// Compile it
			return new RazorViewTemplate(this, compiler.CompileView(), viewfile);
		}

		Type start_view_type = null;
		bool start_view_prepared =false;



		public StartView CreateStartView()
		{
			// Compile the start view
			if (!start_view_prepared)
			{
				start_view_prepared = true;

				// Load the start view
				string start_view_file = Service.Application.MapPath("/Views/_ViewStart.cshtml");
				if (System.IO.File.Exists(start_view_file))
				{
					var compiler = new RazorViewCompiler();
					compiler.BaseClass = typeof(StartView);
					compiler.ViewFile = start_view_file;
					start_view_type = compiler.CompileView();
				}
			}

			// Do we have a start view?
			if (start_view_type == null)
				return null;

			// Create it
			return (StartView)Activator.CreateInstance(start_view_type);
		}

		#endregion
	}
}
			