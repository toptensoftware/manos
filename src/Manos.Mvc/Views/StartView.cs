using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manos.Http;
using System.IO;

namespace Manos.Mvc
{
	public abstract class StartView : ViewBase
	{
		public ViewBase RootView
		{
			get;
			set;
		}

		public new string Layout
		{
			get
			{
				return RootView.Layout;
			}
			set
			{
				RootView.Layout = value;
			}
		}


		public new void DefineSection(string name, Action action)
		{
			RootView.DefineSection(name, action);
		}

		internal void Execute(ViewBase RootView)
		{
			// Setup self
			this.RootView = RootView;
			this.Context = RootView.Context;
			this.Html = RootView.Html;
			this.Output = RootView.Output;

			// Run the template
			OnExecute();

			this.Output = null;
		}
	}
}
