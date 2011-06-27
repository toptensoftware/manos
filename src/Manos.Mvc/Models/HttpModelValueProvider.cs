using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class HttpModelValueProvider : IModelValueProvider
	{
		public HttpModelValueProvider(IManosContext ctx)
		{
			m_Context = ctx;
		}

		private IManosContext m_Context;

		public string GetValue(string key)
		{
			// Posted form data first
			var str = m_Context.Request.PostData.GetString(key);
			if (str != null)
				return str;

			// URI Data second
			str = m_Context.Request.UriData.GetString(key);
			if (str != null)
				return str;

			// Query string third
			str = m_Context.Request.QueryData.GetString(key);
			if (str != null)
				return str;

			// Not found
			return null;
		}
	}
}
