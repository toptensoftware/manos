using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public interface IViewTemplate
	{
		void Render(ControllerContext ctx, object model, View _innerView, bool runStartPage);
	}
}
