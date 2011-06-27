using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Manos.Mvc
{
	public class HtmlHelper
	{
		public HtmlHelper(ControllerContext ctx)
		{
			this.Context = ctx;
		}

		public ControllerContext Context;
		public Dictionary<string, object> HelperData
		{
			get
			{
				if (_HelperData == null)
					_HelperData = new Dictionary<string, object>();
				return _HelperData;
			}
		}
		Dictionary<string, object> _HelperData;

		public HtmlString Raw(string val)
		{
			return new HtmlString(val);
		}

		public TextWriter Output
		{
			get
			{
				return Context.CurrentView.Output;
			}
		}

		public object Model
		{
			get
			{
				return Context.CurrentView.Model;
			}
		}

	}
}
