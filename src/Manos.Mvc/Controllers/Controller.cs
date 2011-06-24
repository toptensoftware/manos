using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Manos.Mvc
{
	public abstract class Controller
	{
		public ControllerContext Context
		{
			get;
			set; 
		}

		public SessionState Session
		{
			get
			{
				return Context.Session;
			}
		}

		public RedirectResult Redirect(string url)
		{
			return new RedirectResult(url);
		}

		public ContentResult Content(string content, string contentType=null, System.Text.Encoding contentEncoding=null)
		{
			return new ContentResult(content, contentType, contentEncoding);
		}

		public ViewResult View(string viewname = null, object model = null)
		{
			if (viewname == null)
			{
				viewname = Context.CurrentAction.Name;
			}
			return new ViewResult(Context.Application, this, viewname, model, false);
		}

		public ViewResult View(object model)
		{
			return new ViewResult(Context.Application, this, Context.CurrentAction.Name, model, false);
		}

		public ViewResult PartialView(string viewname = null, object model = null)
		{
			if (viewname == null)
			{
				viewname = Context.CurrentAction.Name;
			}
			return new ViewResult(Context.Application, this, viewname, model, true);
		}

		public ViewResult PartialView(object model)
		{
			return new ViewResult(Context.Application, this, Context.CurrentAction.Name, model, true);
		}

		public JsonResult Json(object data, string contentType = null, System.Text.Encoding contentEncoding = null)
		{
			return new JsonResult(data, contentType, contentEncoding);
		}
	}

}
