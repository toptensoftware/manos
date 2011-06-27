using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class RedirectResult : ActionResult
	{
		public RedirectResult(string url)
		{
			this.url = url;
		}

		#region IActionResult Members

		public void Process(ControllerContext ctx)
		{
			ctx.Response.Redirect(url);
		}

		#endregion

		string url;
	}
}
