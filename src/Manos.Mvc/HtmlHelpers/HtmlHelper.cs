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

		public HtmlString TextBox(string key, object value=null, object htmlAttributes=null)
		{
			var tag = new TagBuilder("input");

			tag.AddAttribute("type", "text");
			tag.AddAttribute("name", key);
			tag.AddAttribute("id", key);
			tag.AddAttribute("value", ResolveValue(key, value));
			tag.AddAttributes(htmlAttributes);

			return new HtmlString(tag.Render());
		}

		HtmlForm currentForm = null;

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
				View.WriteLiteral("</form>");

				var old = currentForm;
				currentForm = old.outer;
				old.Close();
			}
		}
	}
}
