using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manos.Http;

namespace Manos.Mvc
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class HttpControllerAttribute : Attribute
	{
		public HttpControllerAttribute()
		{

		}

		public HttpControllerAttribute(string pattern)
		{
			this.pattern = pattern;
		}

		public string pattern { get; set; }
	}

}
