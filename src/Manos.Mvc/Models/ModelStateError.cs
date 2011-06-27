using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class ModelStateError
	{
		public ModelStateError(string key, string message)
		{
			Key = key;
			Message = message;
		}
		public string Key { get; set; }
		public string Message { get; set; }
	}
}
