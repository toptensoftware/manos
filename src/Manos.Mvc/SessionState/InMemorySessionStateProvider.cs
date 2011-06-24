using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class InMemorySessionStateProvider : ISessionStateProvider
	{
		const string cookie = "Manos.Mvc.SessionID";

		public void SetCookie(IManosContext ctx, SessionState state)
		{
			// Store the cookie
			ctx.Response.SetCookie(cookie, state.SessionID, TimeSpan.FromDays(7));
		}

		public SessionState LoadSessionState(IManosContext ctx)
		{
			// Get the session cookie
			var id = (string)ctx.Request.Cookies.Get(cookie);
			SessionState state = null;

			// New session?
			if (id==null || !m_ActiveSessions.TryGetValue(id, out state))
			{
				// Allocate a new session
				id = Guid.NewGuid().ToString();
				state = new SessionState(id, 30*60*1000, new Dictionary<string, object>());
				SetCookie(ctx, state);
				return state;
			}

			// Return a copy
			return state.Clone();
		}

		public void SaveSessionState(IManosContext ctx, SessionState state)
		{
			if (!state.Modified)
				return;

			// Just store it
			m_ActiveSessions[state.SessionID] = state;
		}

		Dictionary<string, SessionState> m_ActiveSessions = new Dictionary<string,SessionState>();
	}
}
