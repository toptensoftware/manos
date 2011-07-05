using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Manos.Mvc
{
	public class FileStreamResult : FileResultBase
	{
		public FileStreamResult(Stream data, string contentType, string fileName=null)
			: base(contentType, fileName)
		{
			this.data=data;
		}

		Stream data;

		protected override void OnWriteFileData(ControllerContext ctx)
		{
			data.CopyTo(ctx.Response.Stream);
			ctx.Response.End();
		}
	}
}
