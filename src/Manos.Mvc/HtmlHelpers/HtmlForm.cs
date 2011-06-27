using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class HtmlForm : IDisposable
	{
		public HtmlForm(HtmlHelper helper, HtmlForm outer)
		{
			this.helper = helper;
			this.outer = outer;
		}

		internal HtmlHelper helper;
		internal HtmlForm outer;

		public void Close()
		{
			helper = null;
			outer = null;
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (helper != null)
			{
				helper.EndForm();
				Close();
			}
		}

		#endregion
	}
}
