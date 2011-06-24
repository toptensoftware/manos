using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manos.Http;

namespace Manos.Mvc
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class HttpMethodAttribute : Attribute
	{
		public HttpMethodAttribute(string pattern, params HttpMethod[] methods)
		{
			this.pattern = pattern;
			this.methods = methods;
			this.matchType = null;
		}

		public string pattern;
		public HttpMethod[] methods;
		public Manos.Routing.MatchType? matchType;
	}


}
