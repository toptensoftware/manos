using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class ViewResult : ActionResult
	{
		public ViewResult(MvcApp app, Controller controller, string viewname, object model, bool Partial)
		{
			this.app = app;
			this.viewname = viewname;
			this.controller = controller;
			this.model = model;
			this.partial = Partial;
		}

		#region IActionResult Members

		public void Process(ControllerContext ctx)
		{
			// Find the view file
			var view = app.ViewService.LoadViewTemplate(viewname, controller.GetType().Name);

			// Render it!
			view.Render(ctx, model, null, !partial);

			// Done!
			ctx.Response.End();
		}

		#endregion

		MvcApp app;
		string viewname;
		Controller controller;
		object model;
		bool partial;
	}
}
