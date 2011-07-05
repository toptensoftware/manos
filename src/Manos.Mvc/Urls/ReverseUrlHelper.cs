using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manos.Http;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Manos.Mvc
{
	public static class ReverseUrlHelper
	{
		static Regex rxReplaceVar = new Regex(@"\{([a-zA-Z0-9_\.]+)\}", RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

		public static string GenerateUrl(ControllerContext ctx, Type controllerType, MethodInfo m, HttpMethodAttribute attr, IDictionary<string, object> routeData)
		{
			// Work out the pattern and pattern type
			if (attr.MatchType == Manos.Routing.MatchType.Regex)
			{
				// Regex not supported
				return null;
			}

			var pattern = attr.pattern == null ? m.Name : attr.pattern;

			// Replace all {variables} with value from either routeData or ctx.UriData
			bool AllMatched = true;
			var MatchedVariables = new List<string>();
			var url = rxReplaceVar.Replace(pattern, match=>{

				// Get the name of the variable
				var varName = match.Groups[1].ToString();

				// Remember this variable has been matched
				MatchedVariables.Add(varName);

				// Look it up in the routeData
				if (routeData != null)
				{
					object varValue;
					if (routeData.TryGetValue(varName, out varValue))
					{
						return HttpUtility.UrlEncode(varValue.ToString());
					}
				}

				// Look it up in the UriData
				var unsafeValue = ctx.ManosContext.Request.UriData.Get(varName);
				if (unsafeValue!=null)
				{
					return HttpUtility.UrlEncode(unsafeValue.UnsafeValue);
				}

				AllMatched = false;
				return "";
			});

			// Did we match all route values?
			if (!AllMatched)
				return null;

			var sb = new StringBuilder();

			if (url.StartsWith("/"))
			{
				// Absolute URL
				sb.Append(url);
			}
			else
			{
				// Get the controller path
				var controller_attr = (HttpControllerAttribute)controllerType.GetCustomAttributes(typeof(HttpControllerAttribute), false).FirstOrDefault();
				string controllerPath;
				if (controller_attr==null || controller_attr.pattern==null)
				{
					controllerPath = "/" + MvcApp.CleanControllerName(controllerType.Name);
				}
				else
				{
					controllerPath = controller_attr.pattern;
				}

				sb.Append(controllerPath);
				sb.Append("/");
				sb.Append(url);
			}

			// Append all other routeData variables as query string parameters
			if (routeData!=null)
			{
				bool first = true;
				foreach (var i in routeData)
				{
					if (MatchedVariables.Contains(i.Key))
						continue;
					if (i.Value == null)
						continue;

					if (first)
					{
						sb.Append("?");
						first = false;
					}
					else
						sb.Append("&");

					sb.Append(i.Key);
					sb.Append("=");
					sb.Append(HttpUtility.UrlEncode(i.Value.ToString()));
				}
			}

			return sb.ToString();
		}

		public static string GenerateUrl(ControllerContext ctx, Type controllerType, string targetAction, IDictionary<string, object> routeData, HttpMethod httpMethod = HttpMethod.HTTP_GET)
		{
			foreach (var m in (from i in controllerType.GetMethods() where i.Name == targetAction select i))
			{
				foreach (HttpMethodAttribute attr in m.GetCustomAttributes(typeof(HttpMethodAttribute), false))
				{
					if (attr.methods.Contains(httpMethod))
					{
						var url = GenerateUrl(ctx, controllerType, m, attr, routeData);
						if (url!=null)
							return url;
					}
				}
			}

			throw new InvalidOperationException(string.Format("Unable to match route for controller `{0}` action `{1}` method `{2}'", controllerType.FullName, targetAction, httpMethod));
		}

		public static string GenerateUrl(ControllerContext ctx, Type controllerType, string targetAction, object routeData, HttpMethod httpMethod = HttpMethod.HTTP_GET)
		{
			return GenerateUrl(ctx, controllerType, targetAction, DynamicDictionary.DictionaryFromObject(routeData), httpMethod);
		}
	}
}
