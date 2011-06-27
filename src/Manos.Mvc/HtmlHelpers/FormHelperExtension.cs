using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public static class FormHelperExtension
	{
		public static object ResolveFormValue(this HtmlHelper This, string key, object value)
		{
			// First look for a value from the model state
			ModelValue mv;
			if (This.Context.ModelState.Values.TryGetValue(key, out mv))
			{
				return mv.InputValue;
			}

			// Supplied value
			if (value != null)
				return value;

			// ViewData value
			object view_value;
			if (This.Context.ViewData.TryGetValue(key, out view_value))
				return view_value;

			// Try getting it from the model
			if (This.Model == null)
				return null;
			var pi = This.Model.GetType().GetProperty(key);
			if (pi != null)
				return pi.GetValue(This.Model, null);

			return null;
		}

		public static HtmlString Input(this HtmlHelper This, string type, string key, object value = null, object htmlAttributes = null)
		{
			This.RegisterFormField(key);

			var tag = new TagBuilder("input");

			tag.AddAttribute("type", type);
			tag.AddAttribute("name", key);
			tag.AddAttribute("id", key);
			if (type != "password")
				tag.AddAttribute("value", This.ResolveFormValue(key, value));
			tag.AddAttributes(htmlAttributes);

			if (!This.Context.ModelState.IsFieldValid(key))
				tag.AddClass("model_validation_error");

			return new HtmlString(tag.Render());
		}

		public static HtmlString TextBox(this HtmlHelper This, string key, object value = null, object htmlAttributes = null)
		{
			return This.Input("text", key, value, htmlAttributes);
		}

		public static HtmlString Password(this HtmlHelper This, string key, object value = null, object htmlAttributes = null)
		{
			return This.Input("password", key, value, htmlAttributes);
		}

		public static HtmlString Hidden(this HtmlHelper This, string key, object value = null, object htmlAttributes = null)
		{
			return This.Input("hidden", key, value, htmlAttributes);
		}

		public static HtmlString TextArea(this HtmlHelper This, string key, object value = null, object htmlAttributes = null)
		{
			This.RegisterFormField(key);

			var tag = new TagBuilder("textarea");

			tag.AddAttribute("name", key);
			tag.AddAttribute("id", key);
			tag.AddAttribute("cols", 40);
			tag.AddAttribute("rows", 4);

			tag.AddAttributes(htmlAttributes);
			if (!This.Context.ModelState.IsFieldValid(key))
				tag.AddClass("model_validation_error");

			var text = This.ResolveFormValue(key, value);
			if (text != null)
				tag.AddContent(text.ToString());

			tag.AlwaysUseFullForm = true;

			return new HtmlString(tag.Render());
		}

		public static HtmlString CheckBox(this HtmlHelper This, string key, object value = null, object htmlAttributes = null)
		{
			This.RegisterFormField(key);

			var tag = new TagBuilder("input");

			tag.AddAttribute("type", "checkbox");
			tag.AddAttribute("name", key);
			tag.AddAttribute("id", key);
			tag.AddAttribute("value", "true");
			tag.AddAttributes(htmlAttributes);

			if ((bool)Convert.ChangeType(This.ResolveFormValue(key, value), typeof(bool)))
			{
				tag.AddAttribute("checked", "checked");
			}

			if (!This.Context.ModelState.IsFieldValid(key))
				tag.AddClass("model_validation_error");

			return new HtmlString(tag.Render());
		}

		class FormData
		{
			public string LastFormField;
			public Dictionary<string, int> RadioButtonIndicies;
			public HtmlForm currentForm = null;
			public List<string> FormFields = null;
		}

		static FormData GetFormData(this HtmlHelper This)
		{
			object fd;
			if (!This.HelperData.TryGetValue("form_data", out fd))
			{
				fd = new FormData();
				This.HelperData["form_data"]=fd;
			}
			return (FormData)fd;

		}

		public static HtmlString RadioButton(this HtmlHelper This, string key, object button_value, object form_value = null, object htmlAttributes = null)
		{
			var fd = This.GetFormData();

			if (fd.RadioButtonIndicies==null)
			{
				fd.RadioButtonIndicies = new Dictionary<string, int>();
			}

			int index;
			if (!fd.RadioButtonIndicies.TryGetValue(key, out index))
			{
				index = 0;
			}
			index++;
			fd.RadioButtonIndicies[key] = index;
			string id = key + index.ToString();

			This.RegisterFormField(key);

			form_value = This.ResolveFormValue(key, form_value);

			fd.LastFormField = id;

			var tag = new TagBuilder("input");

			tag.AddAttribute("type", "radio");
			tag.AddAttribute("name", key);
			tag.AddAttribute("id", id);
			tag.AddAttribute("value", button_value);
			tag.AddAttributes(htmlAttributes);

			if (button_value.Equals(form_value))
			{
				tag.AddAttribute("checked", "checked");
			}

			if (!This.Context.ModelState.IsFieldValid(key))
				tag.AddClass("model_validation_error");

			return new HtmlString(tag.Render());
		}

		public static HtmlString Label(this HtmlHelper This, string text, string key = null)
		{
			if (key == null)
			{
				key = This.GetFormData().LastFormField;
			}
			return new HtmlString(string.Format("<label for=\"{0}\">{1}</label>", key, UnsafeString.Escape(text)));
		}

		public static void RegisterFormField(this HtmlHelper This, string key)
		{
			var fd = This.GetFormData();

			fd.LastFormField = key;

			if (fd.currentForm != null)
			{
				if (fd.FormFields == null)
					fd.FormFields = new List<string>();

				if (!fd.FormFields.Contains(key))
					fd.FormFields.Add(key);
			}

		}

		public static HtmlForm BeginForm(this HtmlHelper This)
		{
			var fd = This.GetFormData();

			// Create a new form 
			fd.currentForm = new HtmlForm(This, fd.currentForm);

			var tag = new TagBuilder("form");

			tag.AddAttribute("method", "post");
			tag.AddAttribute("action", This.Context.ManosContext.Request.Path);

			This.Output.Write(tag.RenderOpening());
			This.Output.Write("\n");

			return fd.currentForm;
		}

		public static void EndForm(this HtmlHelper This)
		{
			var fd = This.GetFormData();

			if (fd.currentForm != null)
			{
				if (fd.FormFields != null)
				{
					string fields = string.Join(",", fd.FormFields.ToArray());

					var tag = new TagBuilder("input");
					tag.AddAttribute("type", "hidden");
					tag.AddAttribute("name", "_Manos_Mvc_FormFields");
					tag.AddAttribute("value", md5.Calculate(This.Context.Application.ServerKey + fields) + "/" + fields);

					This.Output.Write(tag.Render());
					This.Output.Write("\n");

					fd.FormFields = null;
				}

				This.Output.Write("</form>");

				var old = fd.currentForm;
				fd.currentForm = old.outer;
				old.Close();
			}
		}

		public static string ValidationSummary(this HtmlHelper This)
		{
			if (This.Context.ModelState.Errors.Count > 0)
			{
				This.Output.Write("<div class\"validation-summary-errors\">\n");
				This.Output.Write(" <ul>\n");
				foreach (var e in This.Context.ModelState.Errors)
				{
					This.Output.Write("  <li>");
					This.Output.Write(UnsafeString.Escape(e.Message));
					This.Output.Write("</li>\n");
				}
				This.Output.Write(" </ul>\n");
				This.Output.Write("</div>\n");
			}

			return string.Empty;
		}
	}
}
