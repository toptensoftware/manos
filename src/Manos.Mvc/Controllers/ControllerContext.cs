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
		public MvcApp Application { get; set; }
		public IManosContext ManosContext { get; set; }
		public MethodInfo CurrentAction { get; set; }
		public Controller Controller { get; set; }

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

		internal void OnPostRequest()
		{
			// Save modified session state
			if (_session != null && _session.Modified)
				Application.SessionStateProvider.SaveSessionState(ManosContext, _session);
		}

	}
}
