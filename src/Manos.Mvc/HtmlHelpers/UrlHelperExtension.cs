using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public static class UrlHelperExtension
	{
		public static HtmlString ActionUrl(this HtmlHelper This, string actionName, Type controllerType, object routeData = null)
		{
			return new HtmlString(ReverseUrlHelper.GenerateUrl(This.Context, controllerType, actionName, routeData));
		}

		public static HtmlString ActionUrl<T>(this HtmlHelper This, string actionName, object routeData = null)
		{
			return new HtmlString(ReverseUrlHelper.GenerateUrl(This.Context, typeof(T), actionName, routeData));
		}

		public static HtmlString ActionUrl(this HtmlHelper This, string actionName, string controllerName, object routeData = null)
		{
			return new HtmlString(ReverseUrlHelper.GenerateUrl(This.Context, ControllerTypeFromName(This, controllerName), actionName, routeData));
		}


		public static HtmlString ActionLink(this HtmlHelper This, string linkText, string actionName, Type controllerType, object routeData = null, object htmlAttributes = null)
		{
			var t = new TagBuilder("a");

			t.AddAttribute("href", ReverseUrlHelper.GenerateUrl(This.Context, controllerType, actionName, routeData));
			t.AddContent(UnsafeString.Escape(linkText));
			t.AddAttributes(htmlAttributes);

			return new HtmlString(t.Render());
		}

		public static HtmlString ActionLink<T>(this HtmlHelper This, string linkText, string actionName, object routeData = null, object htmlAttributes = null) where T : Controller
		{
			return ActionLink(This, linkText, actionName, typeof(T), routeData, htmlAttributes);
		}

		public static HtmlString ActionLink(this HtmlHelper This, string linkText, string actionName, string controllerName = null, object routeData = null, object htmlAttributes = null)
		{
			return ActionLink(This, linkText, actionName, ControllerTypeFromName(This, controllerName), routeData, htmlAttributes);
		}

		public static Type ControllerTypeFromName(this HtmlHelper This, string controllerName)
		{
			Type controllerType = null;
			if (controllerName == null)
			{
				// Use current controller
				controllerType = This.Context.Controller.GetType();
			}
			else
			{
				// Look up controller name
				controllerType = This.Context.Application.GetControllerType(controllerName);
			}

			if (controllerType==null)
				throw new InvalidOperationException(string.Format("Can't find a controller named `{0}`", controllerName));

			// Done
			return controllerType;
		}


	}

}
