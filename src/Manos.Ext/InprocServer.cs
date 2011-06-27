/*
 * Manos.Ext.InprocServer - run Manos as an inproc server to you own exe
 * 
 * Reads config settings from app.config, all settings are optional and default to values shown below
 * 
 
 		<add key="Manos.Port" value="8080"/>
		<add key="Manos.SecurePost" value=""/>
		<add key="Manos.User" value=""/>
		<add key="Manos.IPAddress" value=""/>
		<add key="Manos.CertificateFile" value=""/>
		<add key="Manos.KeyFile" value="">
		<add key="Manos.DocumentRoot" value="">

 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.Configuration;
using Mono.Unix.Native;

namespace Manos.Ext
{
	static class Extensions
	{

		public static int? GetInteger(this NameValueCollection nv, string name)
		{
			string str = nv[name];
			if (str == null)
				return null;

			int val;
			return int.TryParse(str, out val) ? (int?)val : null;
		}

		public static int GetInteger(this NameValueCollection nv, string name, int defaultValue)
		{
			string str = nv[name];
			if (str == null)
				return defaultValue;

			int val;
			return int.TryParse(str, out val) ? val : defaultValue;
		}

		public static string GetString(this NameValueCollection nv, string name, string defaultValue)
		{
			string str = nv[name];
			return str == null ? defaultValue : str;

		}
	}

	public class InprocServer
	{
		public static int Run(string[] args)
		{
			return Run(Assembly.GetCallingAssembly(), args);
		}

		public static int Run(Assembly ApplicationAssembly, string[] args)
		{
			// Load app settings
			var Port = ConfigurationManager.AppSettings.GetInteger("Manos.Port", 8080);
			var SecurePort = ConfigurationManager.AppSettings.GetInteger("Manos.SecurePort");
			var User = ConfigurationManager.AppSettings.GetString("Manos.User", null);
			var IPAddress = ConfigurationManager.AppSettings.GetString("Manos.IPAddress", null);
			var CertificateFile = ConfigurationManager.AppSettings.GetString("Manos.CertificateFile", null);
			var KeyFile = ConfigurationManager.AppSettings.GetString("Manos.KeyFile", null);
			var DocumentRoot = ConfigurationManager.AppSettings.GetString("Manos.DocumentRoot", System.IO.Directory.GetCurrentDirectory());

			string BrowseTo = null;

			// Process arguments
			foreach (var a in args)
			{
				// Args are in format [/-]<switchname>[:<value>];
				if (a.StartsWith("--"))
				{
					string SwitchName = a.Substring(2);
					string Value = null;

					int colonpos = SwitchName.IndexOf(':');
					if (colonpos >= 0)
					{
						// Split it
						Value = SwitchName.Substring(colonpos + 1);
						SwitchName = SwitchName.Substring(0, colonpos);
					}

					switch (SwitchName.ToLower())
					{
						case "documentroot":
							DocumentRoot = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), Value);
							break;

						case "browse":
							BrowseTo = Value;
							if (BrowseTo==null)
								BrowseTo="";
							break;

						default:
							Console.WriteLine("Ignoring unknown switch: {0}", a);
							break;
					}
				}
				else
				{
					Console.WriteLine("Ignoring unknown command line argument: {0}", a);
				}
			}

			System.IO.Directory.SetCurrentDirectory(DocumentRoot);

			// Find the manos app
			var manos_app_type = (from t in ApplicationAssembly.GetTypes() where typeof(ManosApp).IsAssignableFrom(t) select t).FirstOrDefault();
			if (manos_app_type == null)
				throw new InvalidOperationException("No class of type ManosApp found in assembly");

			// Create instance of app
			var app = (ManosApp)Activator.CreateInstance(manos_app_type);

			// Starting
			Console.WriteLine("Running {0} on port {1}.", app, Port);

			// Show working folder
			Console.WriteLine("Document Root: {0}", System.IO.Directory.GetCurrentDirectory());

			// Setup user
			if (User != null)
			{
				PlatformID pid = System.Environment.OSVersion.Platform;
				if (pid != PlatformID.Unix)
				{
					throw new InvalidOperationException("User can not be set on Windows platforms.");
				}

				AppHost.AddTimeout(TimeSpan.Zero, RepeatBehavior.Single, User, DoSetUser);
			}

			// Work out listen address
			var listenAddress = System.Net.IPAddress.Any;
			if (IPAddress != null)
				listenAddress = System.Net.IPAddress.Parse(IPAddress);

			// Start listeneing
			AppHost.ListenAt(new System.Net.IPEndPoint(listenAddress, Port));

			if (SecurePort.HasValue)
			{
				AppHost.InitializeTLS("NORMAL");
				AppHost.SecureListenAt(new System.Net.IPEndPoint(listenAddress, SecurePort.Value), CertificateFile, KeyFile);
				Console.WriteLine("Running {0} on secure port {1}.", app, SecurePort);
			}

			if (BrowseTo != null)
			{
				var hostname = IPAddress == null ? "http://localhost" : "http://" + IPAddress;
				if (Port != 80)
					hostname += ":" + Port.ToString();

				if (BrowseTo == "")
				{
					BrowseTo = hostname;
				}
				if (BrowseTo.StartsWith("/"))
				{
					BrowseTo = hostname + BrowseTo;
				}

				if (!BrowseTo.StartsWith("http://") && !BrowseTo.StartsWith("https://"))
					BrowseTo = "http://" + BrowseTo;

				AppHost.AddTimeout(TimeSpan.FromMilliseconds(10), RepeatBehavior.Single, BrowseTo, DoBrowseTo);
			}

			AppHost.Start(app);

			return 0;
		}

		private static void DoBrowseTo(ManosApp app, object user_data)
		{
			string BrowseTo=user_data as string;
			Console.WriteLine("Launching {0}", BrowseTo);
			System.Diagnostics.Process.Start(BrowseTo);
		}

		private static void DoSetUser(ManosApp app, object user_data)
		{
#if DISABLE_POSIX
		throw new InvalidOperationException ("Attempt to set user on a non-posix build.");
#else
			string user = user_data as string;

			Console.WriteLine("setting user to: '{0}'", user);

			if (user == null)
			{
				AppHost.Stop();
				throw new InvalidOperationException(String.Format("Attempting to set user to null."));
			}

			Passwd pwd = Syscall.getpwnam(user);
			if (pwd == null)
			{
				AppHost.Stop();
				throw new InvalidOperationException(String.Format("Unable to find user '{0}'.", user));
			}

			int error = Syscall.seteuid(pwd.pw_uid);
			if (error != 0)
			{
				AppHost.Stop();
				throw new InvalidOperationException(String.Format("Unable to switch to user '{0}' error: '{1}'.", user, error));
			}

#endif
		}
	}
}
