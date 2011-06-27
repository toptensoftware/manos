using System;
using System.Collections.Generic;
using System.Text;
using Manos.Mvc;

namespace $APPNAME.Controllers
{
	[HttpController]
	class HomeController : Controller
	{
		public HomeController()
		{
		}

		[HttpGet]			// /Home/Index
		[HttpGet("/")]		// Site default page
		public ActionResult Index()
		{
			ViewBag.Message = "This page was rendered through Razor";
			return View();
		}

		[HttpGet]
		public ActionResult RedirectDemo()
		{
			return Redirect("/Home/SimpleContentDemo");
		}

		[HttpGet]
		public ActionResult SimpleContentDemo()
		{
			return Content("Hello World");
		}

		[HttpGet]
		public ActionResult JsonDemo()
		{
			return Json(new
			{
				Apples=1,
				Pears=2,
				Bananas=3,
			});
		}
	}
}
