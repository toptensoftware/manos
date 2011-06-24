using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class JsonResult : IActionResult
	{
		public JsonResult(object data, string contentType, System.Text.Encoding contentEncoding)
		{
			this.data = data;
			this.contentType = contentType;
			this.contentEncoding = contentEncoding;
		}

		#region IActionResult Members

		public void Process(ControllerContext ctx)
		{
			ctx.Response.SetHeader("Content-Type", contentType ?? "application/json");

			if (contentEncoding != null)
			{
				ctx.Response.ContentEncoding = contentEncoding;
				ctx.Response.SetHeader("Content-Encoding", contentEncoding.HeaderName);
			}

			var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
			ctx.Response.Write(serializer.Serialize(data));
			ctx.Response.End();
		}

		#endregion

		object data;
		string contentType;
		System.Text.Encoding contentEncoding;
	}
}
