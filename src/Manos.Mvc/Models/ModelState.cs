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

		public void AddError(string key, string message)
		{
			Errors.Add(new ModelStateError(key, message));
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

		ModelValue GetModelValue(IModelValueProvider provider, PropertyInfo pi)
		{
			// Get the model value
			var string_value = provider.GetValue(pi.Name);
			if (string_value == null)
				return null;

			// Create the model value entry
			var mv = new ModelValue();
			mv.Name = pi.Name;
			mv.InputValue = string_value;

			// Convert to required type
			if (pi.PropertyType != typeof(string))
			{
				try
				{
					mv.ConvertedValue = Convert.ChangeType(string_value, pi.PropertyType);
				}
				catch (Exception x)
				{
					mv.Errors.Add(new ModelStateError(pi.Name, string.Format("Can't convert '{0}' to {1} - {2}", string_value, pi.PropertyType.FullName, x.Message)));
				}
			}
			else
			{
				mv.ConvertedValue = string_value;
			}

			return mv;
		}

		void ApplyModelValue(IModelValueProvider provider, object model, PropertyInfo pi)
		{
			// Get converted value
			var mv = GetModelValue(provider, pi);
			if (mv == null)
				return;

			// Add to our collection of model values
			Values.Add(mv.Name, mv);

			// Copy over errors too
			foreach (var e in mv.Errors)
			{
				Errors.Add(e);
			}

			// Apply the property value
			if (mv.IsValid)
			{
				// Save the original value
				mv.OriginalValue = pi.GetValue(model, null);

				// Apply the new value
				pi.SetValue(model, mv.ConvertedValue, null);
			}
		}

		public bool TryUpdateModel(IModelValueProvider provider, object model)
		{
			// Update all property values
			var ModelType = model.GetType();
			foreach (var pi in ModelType.GetProperties())
			{
				ApplyModelValue(provider, model, pi);
			}

			return IsValid;
		}
	}
}
