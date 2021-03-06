﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

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

		public ModelState ModelState
		{
			get
			{
				return Context.ModelState;
			}
		}

		public Dictionary<string, object> ViewData
		{
			get
			{
				return Context.ViewData;
			}
		}

		public dynamic ViewBag
		{
			get
			{
				return Context.ViewBag;
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

		public bool TryUpdateModel(object model, string[] include_fields=null, string[] exclude_fields=null)
		{
			ModelBinder b = new ModelBinder(ModelState);
			b.IncludeFields = include_fields;
			b.ExcludeFields = exclude_fields;
			return b.TryUpdateModel(new HttpModelValueProvider(Context), model);
		}

		public FileStreamResult File(Stream data, string contentType, string fileName=null)
		{
			return new FileStreamResult(data, contentType, fileName);
		}

		public FileResult File(string sourceFile, string contentType = null, string fileName = null)
		{
			return new FileResult(sourceFile, contentType, fileName);
		}

		public FileBytesResult File(byte[] data, string contentType = null, string fileName = null)
		{
			return new FileBytesResult(data, contentType, fileName);
		}
	}

}
