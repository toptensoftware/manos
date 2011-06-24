using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	class CompileException : Exception
	{
		public CompileException(string message) : 
			base(message)
		{

		}

		public void AddError(string file, int line, int character, int length, string message)
		{
			var e = new Error()
			{
				file = file,
				line = line,
				character = character,
				length = length,
				message = message
			};

			m_Errors.Add(e);
		}

		List<Error> m_Errors = new List<Error>();

		public IEnumerable<Error> Errors
		{
			get
			{
				return m_Errors;
			}
		}

		public class Error
		{
			public string file;
			public int line;
			public int character;
			public int length;
			public string message;
		}
	}
}
