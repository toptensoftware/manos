using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.CodeDom;
using System.Globalization;
using System.Web.Razor;
using System.Web;
using System.Web.Razor.Parser.SyntaxTree;
using System.Dynamic;


namespace Manos.Mvc.DesignTime
{
	[BuildProviderAppliesTo(BuildProviderAppliesTo.Web)]
	public class RazorBuildProvider : BuildProvider
	{
		public RazorBuildProvider()
		{
		}

		RazorViewCompiler _compiler;
		public RazorViewCompiler Compiler
		{
			get
			{ 
				if (_compiler==null)
				{
					_compiler = new RazorViewCompiler();
					_compiler.DesignTimeMode = true;
				}
				return _compiler;
			}
		}

		public override Type GetGeneratedType(System.CodeDom.Compiler.CompilerResults results)
		{
			return results.CompiledAssembly.GetType(string.Format(CultureInfo.CurrentCulture, "{0}.{1}", Compiler.Namespace, Compiler.ClassName));
		}

		public override CompilerType CodeCompilerType
		{
			get
			{
				return GetDefaultCompilerTypeForLanguage(Compiler.Language.LanguageName);
			}
		}

		public override void GenerateCode(AssemblyBuilder assemblyBuilder)
		{
			assemblyBuilder.AddCodeCompileUnit(this, this.GeneratedCode);
			assemblyBuilder.GenerateTypeFactory(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", Compiler.Namespace, Compiler.ClassName));
		}


		CodeCompileUnit _generatedCode;

		private static HttpParseException CreateExceptionFromParserError(RazorError error, string virtualPath)
		{
			return new HttpParseException(error.Message + Environment.NewLine, null, virtualPath, null, error.Location.LineIndex + 1);
		}

		private void EnsureGeneratedCode()
		{
			if (_generatedCode == null)
			{
				GeneratorResults results;
				using (var reader = base.OpenReader())
				{
					Compiler.ViewFile = null;
					results = Compiler.GenerateViewCode(reader);
				}
				if (!results.Success)
				{
					throw CreateExceptionFromParserError(results.ParserErrors.Last<RazorError>(), this.VirtualPath);
				}
				this._generatedCode = results.GeneratedCode;
			}
		}

		internal CodeCompileUnit GeneratedCode
		{
			get
			{
				EnsureGeneratedCode();
				return _generatedCode;
			}
			set
			{
				_generatedCode = value;
			}
		}

	
	}
}
