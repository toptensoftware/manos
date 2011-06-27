using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Manos.Mvc
{
	public class ModelState
	{
		public ModelState()
		{
			Values = new Dictionary<string, ModelValue>();
			Errors = new List<ModelStateError>();
		}

		public Dictionary<string, ModelValue> Values { get; private set; }
		public List<ModelStateError> Errors { get; private set; }

		public void AddModelError(string key, string message)
		{
			var e = new ModelStateError(key, message);

			var mv = GetValue(key);
			if (mv == null)
			{
				mv = new ModelValue();
				mv.Name = key;
				Values.Add(key, mv);
			}

			mv.Errors.Add(e);
			Errors.Add(e);
		}

		public bool IsValid
		{
			get
			{
				return Errors.Count==0;
			}
		}

		public ModelValue GetValue(string key)
		{
			ModelValue mv;
			if (Values.TryGetValue(key, out mv))
				return mv;

			return null;
		}

		public bool IsFieldValid(string key)
		{
			var mv = GetValue(key);
			return mv == null || mv.IsValid;
		}

	}
}
