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

		public RazorViewEngine(MvcApp app)
		{
			this.Application = app;
		}

		MvcApp Application;

		static Type CompileViewFile(string viewfile, Type baseClass)
		{
			// Create langauge
			var language = new CSharpRazorCodeLanguage();

			// Create host
			var host = new RazorEngineHost(language);
			host.DefaultBaseClass = baseClass.FullName;
			host.DefaultClassName = "View";
			host.DefaultNamespace = "Manos.Mvc";
			host.NamespaceImports.Add("System");
			host.NamespaceImports.Add("System.IO");
			host.GeneratedClassContext = new GeneratedClassContext("OnExecute", "Write", "WriteLiteral", "WriteTo", "WriteLiteralTo", "Template", "DefineSection");

			// Create engine
			var eng = new RazorTemplateEngine(host);

			// Generate code from the file
			GeneratorResults results = null;
			using (var rdr = new StreamReader(viewfile))
			{
				//results = eng.GenerateCode(rdr);
				results = eng.GenerateCode(rdr, "View", "Manos.Mvc", viewfile);
			}

			// Check for errors
			if (!results.Success)
			{
				var x = new CompileException("Failed to parse/generate Razor View");
				foreach (var e in results.ParserErrors)
				{
					x.AddError(viewfile, e.Location.LineIndex + 1, e.Location.CharacterIndex + 1, e.Length, e.Message);
				}
				throw x;
			}

			// Compile the code dom
			var compileParams = new CompilerParameters()
			{
				GenerateInMemory = true,
				IncludeDebugInformation = true,
				GenerateExecutable = false,
				CompilerOptions = "/target:library /optimize"
			};

			// Add references to all currently loaded assemblies
			foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (!a.IsDynamic)
					compileParams.ReferencedAssemblies.Add(a.Location);
			}

			// Compile
			var codeDom = new CSharpCodeProvider();
			var compileResults = codeDom.CompileAssemblyFromDom(compileParams, results.GeneratedCode);

			// Display compiler errors
			if (compileResults.Errors != null && compileResults.Errors.Count > 0)
			{
				var x = new CompileException("Failed to compile Razor view");
				foreach (CompilerError e in compileResults.Errors)
				{
					x.AddError(viewfile, e.Line, e.Column, 0, e.ErrorText);
				}
				throw x;
			}

			return compileResults.CompiledAssembly.GetType("Manos.Mvc.View");
		}


		public IViewTemplate CreateView(string viewfile)
		{
			// Is it a razor view file?
			if (!viewfile.EndsWith(".cshtml", StringComparison.InvariantCultureIgnoreCase))
				return null;

			// Compile it
			return new RazorViewTemplate(this, CompileViewFile(viewfile, typeof(ViewBase)), viewfile);
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
				string start_view_file = Application.MapPath("/Views/_ViewStart.cshtml");
				if (System.IO.File.Exists(start_view_file))
				{
					start_view_type = CompileViewFile(start_view_file, typeof(StartView));
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
			