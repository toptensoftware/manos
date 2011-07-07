using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public interface ActionResult
	{
		void Process(ControllerContext ctx);
	}
}
