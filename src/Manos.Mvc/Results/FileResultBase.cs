using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Manos.Mvc
{
	public abstract class FileResultBase : ActionResult
	{
		public FileResultBase(string contentType = null, string fileName = null)
		{
			this.contentType = contentType;
			this.fileName = fileName;
		}

		#region IActionResult Members

		public void Process(ControllerContext ctx)
		{
			// Work out content type from fileName
			if (contentType==null && fileName!=null)
				contentType = ManosMimeTypes.GetMimeType(fileName);

			// Setup content type
			if (contentType != null)
			{
				ctx.Response.Headers.SetNormalizedHeader("Content-Type", contentType);
			}

			// Setup content disposition
			if (fileName != null)
			{
				ctx.Response.Headers.SetNormalizedHeader("Content-Disposition", "attachment; filename=" + fileName);
			}

			OnWriteFileData(ctx);
		}

		#endregion

		string contentType;
		string fileName;

		protected abstract void OnWriteFileData(ControllerContext ctx);
	}
}
