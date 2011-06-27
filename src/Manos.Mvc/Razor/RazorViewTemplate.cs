using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class RazorViewTemplate : IViewTemplate
	{
		public RazorViewTemplate(RazorViewEngine owner, Type t, string viewfile)
		{
			// Check the view derives from ViewBase
			if (!typeof(ViewBase).IsAssignableFrom(t))
			{
				throw new Exception(string.Format("View does not inherit from {0}", typeof(ViewBase).FullName));
			}

			this.Owner = owner;
			this.ViewType = t;
			this.ViewFile = viewfile;
		}

		RazorViewEngine Owner;
		Type ViewType;
		string ViewFile;

		#region IView Members

		void CheckDoesModelTypeMatch(Type type, Type modelType)
		{
			if (type.IsGenericType)
			{
				if (type.GetGenericTypeDefinition() == typeof(ViewBase<>))
				{
					if (modelType == null)
					{
						throw new InvalidOperationException(string.Format("The strongly typed view {0} expects a model type of {1} but was passed null.", ViewFile, type.GetGenericArguments()[0]));
					}
					if (!type.GetGenericArguments()[0].IsAssignableFrom(modelType))
					{
						throw new InvalidOperationException(string.Format("The strongly typed view {0} expects a model type of {1} but was passed model of type {2}.", ViewFile, type.GetGenericArguments()[0], modelType));
					}

					return;
				}
			}

			if (type == typeof(ViewBase))
				return;

			// Recurse
			CheckDoesModelTypeMatch(type.BaseType, modelType);
		}

		public void Render(ControllerContext ctx, object model, ViewBase innerView, bool runStartPage)
		{
			// Check model type matches on strongly typed views
			CheckDoesModelTypeMatch(ViewType, model==null ? null : model.GetType());

			// Create the view
			var viewbase = (ViewBase)Activator.CreateInstance(ViewType);

			// Pass it the view context
			viewbase.Context = ctx;
			viewbase.Model = model;
			viewbase.Html = new HtmlHelper(ctx, model);
			viewbase.InnerView = innerView;
			viewbase.StartPage = runStartPage ? Owner.CreateStartView() : null;			

			// Execute it!
			viewbase.Execute();
		}

		#endregion
	}
}
