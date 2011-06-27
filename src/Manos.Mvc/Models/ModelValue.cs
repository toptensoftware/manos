using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class ModelValue
	{
		public ModelValue()
		{
			Errors = new List<ModelStateError>();
		}

		public string Name { get; set; }
		public string InputValue { get; set; }
		public object ConvertedValue { get; set; }
		public object OriginalValue { get; set; }
		public List<ModelStateError> Errors { get; private set; }
		public bool IsValid
		{
			get
			{
				return Errors.Count == 0;
			}
		}
	}
}
