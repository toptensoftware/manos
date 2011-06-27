using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manos.Mvc
{
	public static class md5
	{
		public static string Calculate(byte[] data)
		{
			if (data == null)
				return "";

			System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
			var bs = x.ComputeHash(data);
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			foreach (byte b in bs)
			{
				s.Append(b.ToString("x2").ToLower());
			}
			return s.ToString();
		}

		public static string Calculate(string input)
		{
			if (input == null)
				input = "";
			System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
			bs = x.ComputeHash(bs);
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			foreach (byte b in bs)
			{
				s.Append(b.ToString("x2").ToLower());
			}
			return s.ToString();
		}

		public static string CalculateAscii(string input)
		{
			if (input == null)
				input = "";
			System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] bs = System.Text.Encoding.ASCII.GetBytes(input);
			bs = x.ComputeHash(bs);
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			foreach (byte b in bs)
			{
				s.Append(b.ToString("x2").ToLower());
			}
			string password = s.ToString();
			return password;
		}

	}
}
