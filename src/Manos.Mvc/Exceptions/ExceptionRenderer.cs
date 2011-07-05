using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Manos.Http;

namespace Manos.Mvc
{
	public static class ExceptionRenderer
	{
		public static void RenderLocation(IHttpResponse r, string file, int line)
		{
			if (file == null)
				return;

			// Does the file exist?
			if (System.IO.File.Exists(file))
			{
				try
				{
					var rdr = new StreamReader(file);

					var sb = new StringBuilder();
					r.Write("<pre>");
					for (int i = 1; ; i++)
					{
						if (i > line + 2)
							break;

						var text = rdr.ReadLine();
						if (text == null)
							break;

						if (i < line - 2)
							continue;

						if (i == line)
							r.Write("<span style=\"color:red;\">");

						r.Write("Line {0:0000}: ", i);
						r.WriteLine(UnsafeString.Escape(text));

						if (i == line)
							r.Write("</span>");
					}

					r.WriteLine("</pre>");
				}
				catch (Exception)
				{
					// Ignore
				}
			}

			r.Write("<p><b>Location:</b> {0} <b>Line:</b> {1}</p>\n", UnsafeString.Escape(file), line);
		}

		public static void RenderException(IManosContext ctx, Exception x, bool debug)
		{
			var r = ctx.Response;

			r.StatusCode = 500;

			// Header
			r.Write(@"<html>
<head>
<title>{0}</title>
<style>
	body {{font-family:Sans-Serif; font-size:10pt; }}
	h1 {{ color:red; border-bottom: 1px solid silver; font-size:14pt; }}
	h2 {{ color:maroon; font-style:italic; font-size:12pt; }}
	pre {{ background-color:#ffffcc; }}
</style>
</head>
<body>
	<h1>Server Error</h1>
	<h2>{0}</h2>
	<p><b>Exception Details:</b> {1}: {0}</p>
", UnsafeString.Escape(x.Message), UnsafeString.Escape(x.GetType().ToString()));

			var ce = x as CompileException;
			if (ce == null)
			{
				// Location info
				try
				{
					// Get stack trace for the exception with source file information
					var st = new StackTrace(x, true);
					var frame = st.GetFrame(0);
					RenderLocation(r, frame.GetFileName(), frame.GetFileLineNumber());
				}
				catch (Exception)
				{
					// Ignore
				}
			}
			else
			{
				foreach (var e in ce.Errors)
				{
					r.Write("<h2>" + UnsafeString.Escape(e.message) + "</h2>\n");
					RenderLocation(r, e.file, e.line);
				}
			}

			// Stack track
			r.Write(@"
	<p><b>Stack Trace:</b></p>
	<pre>{0}</pre>
",UnsafeString.Escape(x.StackTrace));

			// Footer
			r.Write(@"
</body>
</html>");

			r.End();

		}
	}
}
