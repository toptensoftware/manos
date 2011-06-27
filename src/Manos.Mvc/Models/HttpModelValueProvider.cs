using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class HttpModelValueProvider : IModelValueProvider
	{
		public HttpModelValueProvider(ControllerContext ctx)
		{
			m_Context = ctx;
		}

		private ControllerContext m_Context;

		public string GetValue(string key)
		{
			// Posted form data first
			var str = m_Context.ManosContext.Request.PostData.Get(key).UnsafeValue;
			if (str != null)
				return str;

			// URI Data second
			str = m_Context.ManosContext.Request.UriData.GetString(key);
			if (str != null)
				return str;

			// Query string third
			str = m_Context.ManosContext.Request.QueryData.GetString(key);
			if (str != null)
				return str;

			// Not found
			return null;
		}

		public string[] GetFieldList(object model)
		{
			// Get the form field that lists the used fields
			string hashed_field_list = GetValue("_Manos_Mvc_FormFields");
			if (hashed_field_list != null)
			{
				// Split between the hash and the field list
				int slashpos = hashed_field_list.IndexOf('/');
				if (slashpos >= 0)
				{
					// Check the hash matches
					var hash = hashed_field_list.Substring(0, slashpos);
					var field_list = hashed_field_list.Substring(slashpos + 1);
					if (md5.Calculate(m_Context.Application.ServerKey + field_list)==hash)
					{
						// All good
						return field_list.Split(',');
					}
				}
			}

			// Something up
			return null;
		}

	}
}
