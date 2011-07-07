using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Manos.Http;

namespace Manos.Mvc
{
	public class ControllerContext
	{
		public ControllerContext()
		{
			ViewData = new Dictionary<string, object>();
			ViewBag = new DynamicDictionary(ViewData);
			ModelState = new ModelState();
		}

		public MvcApp Application { get; set; }
		public IManosContext ManosContext { get; set; }
		public MethodInfo CurrentAction { get; set; }
		public Controller Controller { get; set; }
		public dynamic ViewBag { get; private set; }
		public Dictionary<string, object> ViewData { get; private set; }
		public ModelState ModelState { get; private set; }
		public View CurrentView { get; set; }

		public IHttpResponse Response
		{
			get
			{
				return ManosContext.Response;
			}
		}

		SessionState _session;
		public SessionState Session
		{
			get
			{
				if (_session == null)
					_session = Application.SessionStateProvider.LoadSessionState(ManosContext);
				return _session;
			}
		}

		public void ProcessResult(object result)
		{
			// Process the action result
			ActionResult action_result = result as ActionResult;
			if (action_result != null)
			{
				// Process action result
				action_result.Process(this);
			}
			else
			{
				// Other types
				var string_result = result as string;
				if (string_result != null)
				{
					ManosContext.Response.End(string_result);
				}
				else
				{
					if (result != null)
						ManosContext.Response.End(result.ToString());
					else
						ManosContext.Response.End();
				}
			}

			// Save modified session state
			if (_session != null && _session.Modified)
				Application.SessionStateProvider.SaveSessionState(ManosContext, _session);
		}
	}
}
