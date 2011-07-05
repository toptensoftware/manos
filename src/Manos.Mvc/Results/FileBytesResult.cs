using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class FileBytesResult : FileResultBase
	{
		public FileBytesResult(byte[] data, string contentType=null, string fileName=null)
			: base(contentType, fileName)
		{
			this.data=data;
		}

		byte[] data;

		protected override void OnWriteFileData(ControllerContext ctx)
		{
			ctx.Response.Write(data);
			ctx.Response.End();
		}
	}
}
