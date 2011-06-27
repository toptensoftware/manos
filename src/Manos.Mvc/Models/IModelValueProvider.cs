using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Manos.Mvc
{
	public interface IModelValueProvider
	{
		string GetValue(string key);
	}
}
