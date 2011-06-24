using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manos.Http;

namespace Manos.Mvc
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class HttpGetAttribute : HttpMethodAttribute
	{
		public HttpGetAttribute()
			: base(null, HttpMethods.GetMethods)
		{
		}

		public HttpGetAttribute(string pattern)
			: base(pattern, HttpMethods.GetMethods)
		{
		}
	}

}
