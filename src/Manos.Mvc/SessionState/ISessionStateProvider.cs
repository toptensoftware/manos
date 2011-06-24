using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public interface ISessionStateProvider
	{
		SessionState LoadSessionState(IManosContext ctx);
		void SaveSessionState(IManosContext ctx, SessionState state);
	}
}
