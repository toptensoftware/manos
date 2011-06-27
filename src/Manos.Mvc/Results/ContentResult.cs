using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class ContentResult : ActionResult
	{
		public ContentResult(string content, string contentType, System.Text.Encoding contentEncoding)
		{
			this.content = content;
			this.contentType = contentType;
			this.contentEncoding = contentEncoding;
		}

		#region IActionResult Members

		public void Process(ControllerContext ctx)
		{
			if (contentType!=null)
				ctx.Response.SetHeader("Content-Type", contentType);

			if (contentEncoding != null)
			{
				ctx.Response.ContentEncoding = contentEncoding;
				ctx.Response.SetHeader("Content-Encoding", contentEncoding.HeaderName);
			}

			ctx.Response.End(content);
		}

		#endregion

		string content;
		string contentType;
		System.Text.Encoding contentEncoding;
	}
}
