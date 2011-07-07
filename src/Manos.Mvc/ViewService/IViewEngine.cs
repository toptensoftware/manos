using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public interface IViewEngine
	{
		IViewTemplate CreateView(string viewfile);
	}
}
