using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.IO;
using System.CodeDom.Compiler;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;
using System.Web.Razor.Parser;

namespace Manos.Mvc
{
	public class RazorViewCompiler
	{
		public RazorViewCompiler()
		{
			Language = new CSharpRazorCodeLanguage();
			ClassName = "GeneratedView";
			Namespace = "Manos.Mvc";
		}

		public string ClassName
		{
			get;
			set;
		}

		public string Namespace
		{
			get;
			set;
		}

		public RazorCodeLanguage Language
		{
			get;
			set;
		}

		public Type BaseClass
		{
			get;
			set;
		}

		public string ViewFile
		{
			get;
			set;
		}

		public bool DesignTimeMode
		{
			get;
			set;
		}

		public GeneratorResults GenerateViewCode(TextReader reader)
		{
			// Create host
			var host = new RazorEngineHost(Language);
			host.DefaultBaseClass = BaseClass.FullName;
			host.DefaultClassName = ClassName;
			host.DefaultNamespace = Namespace;
			host.NamespaceImports.Add("System");
			host.NamespaceImports.Add("System.IO");
			host.GeneratedClassContext = new GeneratedClassContext("OnExecute", "Write", "WriteLiteral", "WriteTo", "WriteLiteralTo", "Template", "DefineSection");
			host.DesignTimeMode = this.DesignTimeMode;

			// Create engine
			var eng = new RazorTemplateEngine(host);

			// Generate code from the file
			return eng.GenerateCode(reader, ClassName, Namespace, ViewFile);
		}

		public Type CompileView()
		{
			// generate code
			GeneratorResults results;
			using (var rdr = new StreamReader(ViewFile))
			{
				results =  GenerateViewCode(rdr);
			}

			// Check for errors
			if (!results.Success)
			{
				var x = new CompileException("Failed to parse/generate Razor View");
				foreach (var e in results.ParserErrors)
				{
					x.AddError(ViewFile, e.Location.LineIndex + 1, e.Location.CharacterIndex + 1, e.Length, e.Message);
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
			var codeDom = (CodeDomProvider)Activator.CreateInstance(Language.CodeDomProviderType);
			var compileResults = codeDom.CompileAssemblyFromDom(compileParams, results.GeneratedCode);

			// Display compiler errors
			if (compileResults.Errors != null && compileResults.Errors.Count > 0)
			{
				var x = new CompileException("Failed to compile Razor view");
				foreach (CompilerError e in compileResults.Errors)
				{
					x.AddError(ViewFile, e.Line, e.Column, 0, e.ErrorText);
				}
				throw x;
			}

			return compileResults.CompiledAssembly.GetType("Manos.Mvc.GeneratedView");
		}



	}
}
