using System;
using System.Collections.Generic;
using System.Text;
using Manos.Mvc;

namespace $APPNAME
{
	public class $APPNAME : MvcApp
	{
		public $APPNAME()
		{
			RouteStaticContent("/Content/");
			RegisterAllControllers();
		}
	}
}
