using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class FileResult : FileResultBase
	{
		public FileResult(string sourceFile, string contentType=null, string fileName=null)
			: base(contentType, fileName==null ? System.IO.Path.GetFileName(sourceFile) : fileName)
		{
			this.sourceFile = sourceFile;
		}

		string sourceFile;

		protected override void OnWriteFileData(ControllerContext ctx)
		{
			ctx.Response.SendFile(sourceFile);
		}
	}
}
