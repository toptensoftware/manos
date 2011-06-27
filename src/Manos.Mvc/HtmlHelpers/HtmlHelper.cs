using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class HtmlHelper
	{
		public HtmlHelper(ControllerContext ctx, object Model)
		{
			this.ctx = ctx;
			this.Model = Model;
		}

		ControllerContext ctx;
		object Model;

		public ViewBase View
		{
			get;
			set;
		}

		public HtmlString Raw(string val)
		{
			return new HtmlString(val);
		}

		public object ResolveValue(string key, object value)
		{
			// First look for a value from the model state
			ModelValue mv;
			if (ctx.ModelState.Values.TryGetValue(key, out mv))
			{
				return mv.InputValue;
			}

			// Supplied value
			if (value != null)
				return value;

			// ViewData value
			object view_value;
			if (ctx.ViewData.TryGetValue(key, out view_value))
				return view_value;

			// Try getting it from the model
			if (Model == null)
				return null;
			var pi = Model.GetType().GetProperty(key);
			if (pi != null)
				return pi.GetValue(Model, null);

			return null;
		}

		public HtmlString Input(string type, string key, object value = null, object htmlAttributes = null)
		{
			RegisterFormField(key);

			var tag = new TagBuilder("input");

			tag.AddAttribute("type", type);
			tag.AddAttribute("name", key);
			tag.AddAttribute("id", key);
			if (type!="password")
				tag.AddAttribute("value", ResolveValue(key, value));
			tag.AddAttributes(htmlAttributes);

			if (!ctx.ModelState.IsFieldValid(key))
				tag.AddClass("model_validation_error");

			return new HtmlString(tag.Render());
		}

		public HtmlString TextBox(string key, object value = null, object htmlAttributes = null)
		{
			return Input("text", key, value, htmlAttributes);
		}

		public HtmlString Password(string key, object value = null, object htmlAttributes = null)
		{
			return Input("password", key, value, htmlAttributes);
		}

		public HtmlString Hidden(string key, object value = null, object htmlAttributes = null)
		{
			return Input("hidden", key, value, htmlAttributes);
		}

		HtmlForm currentForm = null;
		List<string> FormFields = null;

		public void RegisterFormField(string key)
		{
			if (currentForm != null)
			{
				if (FormFields == null)
					FormFields = new List<string>();

				FormFields.Add(key);
			}

		}

		public HtmlForm BeginForm()
		{
			// Create a new form 
			currentForm = new HtmlForm(this, currentForm);

			var tag = new TagBuilder("form");

			tag.AddAttribute("method", "post");
			tag.AddAttribute("action", ctx.ManosContext.Request.Path);

			View.WriteLiteral(tag.RenderOpening());
			View.WriteLiteral("\n");

			return currentForm;
		}

		public void EndForm()
		{
			if (currentForm != null)
			{
				if (FormFields != null)
				{
					string fields = string.Join(",", FormFields.ToArray());

					var tag = new TagBuilder("input");
					tag.AddAttribute("type", "hidden");
					tag.AddAttribute("name", "_Manos_Mvc_FormFields");
					tag.AddAttribute("value", md5.Calculate(ctx.Application.ServerKey + fields) + "/" + fields);

					View.WriteLiteral(tag.Render());
					View.WriteLiteral("\n");

					FormFields = null;
				}

				View.WriteLiteral("</form>");

				var old = currentForm;
				currentForm = old.outer;
				old.Close();
			}
		}

		public string ValidationSummary()
		{
			if (ctx.ModelState.Errors.Count > 0)
			{
				View.WriteLiteral("<div class\"validation-summary-errors\">\n");
				View.WriteLiteral(" <ul>\n");
				foreach (var e in ctx.ModelState.Errors)
				{
					View.WriteLiteral("  <li>"); 
					View.Write(e.Message); 
					View.WriteLiteral("</li>\n");
				}
				View.WriteLiteral(" </ul>\n");
				View.WriteLiteral("</div>\n");
			}

			return string.Empty;		}
	}
}
