using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manos.Http;

namespace Manos.Mvc
{
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = false)]
	public class UseThreadPoolAttribute : Attribute
	{
		public UseThreadPoolAttribute()
		{
			this.UseThreadPool = true;
		}

		public UseThreadPoolAttribute(bool UseThreadPool)
		{
			this.UseThreadPool = UseThreadPool;
		}

		public bool UseThreadPool { get; set; }
	}

}
