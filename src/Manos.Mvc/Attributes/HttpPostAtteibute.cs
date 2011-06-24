using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manos.Http;

namespace Manos.Mvc
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class HttpPostAttribute : HttpMethodAttribute
	{
		public HttpPostAttribute()
			: base(null, HttpMethods.PostMethods)
		{
		}

		public HttpPostAttribute(string pattern)
			: base(pattern, HttpMethods.PostMethods)
		{
		}
	}

}
