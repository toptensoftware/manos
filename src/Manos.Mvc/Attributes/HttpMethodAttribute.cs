using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manos.Http;

namespace Manos.Mvc
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class HttpMethodAttribute : Attribute
	{
		public HttpMethodAttribute(string pattern, params HttpMethod[] methods)
		{
			this.pattern = pattern;
			this.methods = methods;
			this.matchType = null;
		}

		public string pattern;
		public HttpMethod[] methods;
		public Manos.Routing.MatchType? matchType;

		public Manos.Routing.MatchType MatchType
		{
			get
			{
				return matchType ?? GuessMatchType(pattern);
			}
		}

		static Manos.Routing.MatchType GuessMatchType(string pattern)
		{
			if (pattern != null)
			{
				if (pattern.IndexOfAny(Manos.Routing.MatchOperationFactory.REGEX_CHARS) >= 0)
					return Manos.Routing.MatchType.Regex;
				if (pattern.IndexOfAny(Manos.Routing.MatchOperationFactory.SIMPLE_CHARS) >= 0)
					return Manos.Routing.MatchType.Simple;
			}
			return Manos.Routing.MatchType.String;
		}


	}


}
