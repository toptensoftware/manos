using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public class TagBuilder
	{
		public TagBuilder(string tagName)
		{
			Name = tagName;

		}

		public string Name
		{
			get;
			set;
		}

		public bool AlwaysUseFullForm
		{
			get;
			set;
		}

		public Dictionary<string, string> Attributes = new Dictionary<string,string>();

		public void AddAttribute(string name, object value)
		{
			Attributes.Add(name, value==null ? null : value.ToString());
		}

		public void AddClass(string className)
		{
			string old;
			if (Attributes.TryGetValue("class", out old) && old!=null)
			{
				Attributes["class"] = old + " " + className;
			}
			else
			{
				Attributes["class"] = className;
			}
		}

		public void AddContent(string content)
		{
			if (m_Content == null)
				m_Content = new StringBuilder();

			m_Content.Append(content);
		}

		void RenderOpening(StringBuilder sb)
		{
			sb.Append("<");
			sb.Append(Name);
			foreach (var a in Attributes)
			{
				sb.Append(" ");
				sb.Append(a.Key);
				if (a.Value != null)
				{
					sb.Append("=");
					sb.Append("\"");
					sb.Append(UnsafeString.Escape(a.Value));
					sb.Append("\"");
				}
			}

		}

		public string RenderOpening()
		{
			var sb = new StringBuilder();
			RenderOpening(sb);
			sb.Append(">");
			return sb.ToString();
		}

		void RenderClosing(StringBuilder sb)
		{
			sb.Append("</");
			sb.Append(Name);
			sb.Append(">");
		}

		public string RenderClosing()
		{
			var sb = new StringBuilder();
			RenderClosing(sb);
			return sb.ToString();
		}

		public string Render()
		{
			var sb = new StringBuilder();

			RenderOpening(sb);

			if ((m_Content == null || m_Content.Length == 0) && !AlwaysUseFullForm)
			{
				sb.Append(" />");
			}
			else
			{
				sb.Append(">");
				sb.Append(m_Content);
				RenderClosing(sb);
			}

			return sb.ToString();
		}

		public void AddAttributes(object attributes)
		{
			if (attributes == null)
				return;

			foreach (var pi in attributes.GetType().GetProperties())
			{
				AddAttribute(pi.Name, pi.GetValue(attributes, null).ToString());
			}
		}

		StringBuilder m_Content;

	}
}
