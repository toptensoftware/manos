using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manos.Http;
using System.IO;

namespace Manos.Mvc
{
	public abstract class View
	{
		public ControllerContext Context
		{
			get;
			set;
		}

		public Dictionary<string, object> ViewData
		{
			get
			{
				return Context.ViewData;
			}
		}

		public dynamic ViewBag
		{
			get
			{
				return Context.ViewBag;
			}
		}


		public HtmlHelper Html
		{
			get;
			set;
		}

		public IHttpResponse Response
		{
			get
			{
				return Context.ManosContext.Response;
			}
		}

		public IHttpRequest Request
		{
			get
			{
				return Context.ManosContext.Request;
			}
		}

		TextWriter _output;
		public TextWriter Output
		{
			get
			{
				return _output;
			}

			set
			{
				_output = value;
			}
		}

		public object Model
		{
			get;
			set;
		}

		public void Write(object o)
		{
			if (o == null)
				return;

			var hs = o as HtmlString;
			if (hs != null)
				Output.Write(hs.Value);
			else
				Output.Write(UnsafeString.Escape(o.ToString()));
		}

		public void WriteLiteral(string literal)
		{
			if (literal == null)
				return;

			Output.Write(literal);
		}

		public static void WriteLiteralTo(TextWriter writer, object content)
		{
			if (content == null)
				return;

			writer.Write(content);
		}

		public static void WriteTo(TextWriter writer, object content)
		{
			if (content == null)
				return;
			var hs = content as HtmlString;
			if (hs != null)
				writer.Write(hs.Value);
			else
				writer.Write(UnsafeString.Escape(content.ToString()));
		}

		Dictionary<string, Action> _sections = null;
		Dictionary<string, Action> Sections
		{
			get
			{
				if (_sections == null)
					_sections = new Dictionary<string, Action>();
				return _sections;
			}
		}


		// Called by Razor when a section is defined
		public void DefineSection(string name, Action action)
		{
			Sections[name] = action;
		}

		public View InnerView
		{
			get;
			set;
		}

		static Action FindSection(View view, string name)
		{
			if (view==null)
				return null;

			Action section;
			if (view.Sections.TryGetValue(name, out section))
				return section;

			// Recurse
			return FindSection(view.InnerView,name);
		}


		// Called by layout pages to render a section from the primary view
		public string RenderSection(string name, bool optional = false)
		{
			// Recursive search for the inner section
			Action section=FindSection(InnerView, name);
			if (section==null)
			{
				if (optional)
					return String.Empty;

				throw new InvalidOperationException(string.Format("No section named `{0}`", name));
			}

			// Save the inner view's output, render the section and restore the output
			var save = InnerView.Output;
			InnerView.Output = Output;
			section();
			InnerView.Output = save;

			// Return empty string to prevent output
			return string.Empty;
		}

		public bool IsSectionDefined(string name)
		{
			return FindSection(InnerView, name) != null;
		}

		public string RenderBody()
		{
			if (InnerView == null)
				return "";

			Output.Write(InnerView.GeneratedBody);
			return "";
		}

		public void Execute()
		{
			// Setup the output to a string builder
			var sb = new StringBuilder();
			Output = new StringWriter(sb);

			// Run the start page
			if (StartPage != null)
				StartPage.Execute(this);

			Context.CurrentView = this;

			try
			{
				// Execute the page view
				OnExecute();
			}
			finally
			{
				Context.CurrentView = null;
			}

			// Clean up the output writer
			Output.Flush();
			Output = null;

			// Capture the output
			GeneratedBody = sb.ToString();

			// Apply layout view
			if (Layout != null)
			{
				IViewTemplate layoutView = Context.Application.LoadViewTemplate(Context.Application.MapPath(Layout));
				layoutView.Render(Context, Model, this, false);
			}
			else
			{
				// Write the output
				Response.Write(GeneratedBody);
			}
		}

		public StartView StartPage
		{
			get;
			set;
		}


		public string Layout
		{
			get;
			set;
		}

		public string GeneratedBody
		{
			get;
			set;
		}


	
		public abstract void OnExecute();
	}



	public abstract class View<T> : View
	{
		public new T Model 
		{
			get
			{
				return (T)base.Model;
			}
		}
	}
}
