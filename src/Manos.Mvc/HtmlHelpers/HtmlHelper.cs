using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class HtmlHelper
	{
		public HtmlHelper(ControllerContext ctx)
		{

		}

		public HtmlString Raw(string val)
		{
			return new HtmlString(val);
		}
	}
}
