//
// Copyright (C) 2010 Jackson Harper (jackson@manosdemono.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//



using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using NDesk.Options;

namespace Manos.Tool
{
	class Driver
	{
		
		public static readonly string COMPILED_TEMPLATES_ASSEMBLY = "CompiledTemplates.dll";
		public static readonly string TEMPLATES_DIRECTORY = "Templates";
		public static readonly string DEPLOYMENT_DIRECTORY = "Deployment";
		
		private static Environment Environment = new Environment ();

		private static StreamWriter output;
		
		public static int Main (string[] args)
		{
			args = ParseGlobalOptions (args);
			
			bool help = false;
			Func<IList<string>, int> command = null;
			
			var p = new OptionSet () {
				{ "h|?|help", v => help = v != null },
				{ "init|i", v => command = Init },
				{ "server|s", v => command = Server },
				{ "docs|d", v => command = Docs },
				{ "build|b", v => command = Build },
				{ "show-environment|se", v => command = ShowEnvironment },
				{ "run|r=", v => command = a => { return Run(v, a); } },
			};
			
			List<string> extra = null;
			try {
				extra = p.Parse(args);
			} catch (OptionException){
				Console.WriteLine ("Try `manos --help' for more information.");
				return 1;
			}
			
			if (help) {
				ShowHelp (p);
				return 0;
			}
			
			if (command == null) {
				ShowHelp (p);
				return 1;
			}
			
			command (extra);
			
			return 0;
		}

		private static StreamWriter StreamForFile (string file)
		{
                        if (file == null)
                           throw new ArgumentNullException ("file");

                        FileStream fs = new FileStream (file, FileMode.Create);
                        StreamWriter sw = new StreamWriter (fs);
			sw.AutoFlush = true;

			return sw;
		}

		private static void SetOutput (string file)
		{
			Console.WriteLine ("setting output:  " + file);
			output = StreamForFile (file);
			Console.SetOut (output);
			Console.SetError (output);
		}

		private static string [] ParseGlobalOptions (string [] args)
		{
			var p = new OptionSet () {
				{ "-data-dir=", v => Environment.DataDirectory = v },
				{ "-out|out=", v => SetOutput (v) },
			};
			
			List<string> extra = null;
			try {
				extra = p.Parse(args);
			} catch (OptionException){
				Console.WriteLine ("Try `manos --help' for more information.");
				return null;
			}
			
			if (extra == null)
				return null;
			
			return extra.ToArray ();
		}
		
		private static int Init (IList<string> args)
		{
			string layout = null;
			var p = new OptionSet() {
				{ "-l|layout=", v => layout = v },
			};
			
			
			Driver d = new Driver ();
			
			try {

				List<string> extra = p.Parse(args);

				if (extra.Count < 1)
				{
					Console.WriteLine("manos [--layout=<LayoutName>] --init <AppName>");
					Console.WriteLine("This will initialize a new application with the supplied name.");
				}

				if (layout == null)
					layout = "default";
				string appname = extra[0];

				Console.WriteLine("initing: {0} with layout {1}", appname, layout);

				InitCommand initer = new InitCommand(Environment, appname);
				initer.Layout = layout;

				initer.Run();
			}
			catch (Exception e)
			{
				Console.WriteLine ("error while initializing application:");
				Console.WriteLine (e);
				return 1;
			}
			
			return 0;
		}

		private static int Server (IList<string> args)
		{
			Driver d = new Driver ();

			
			try {
				d.RunServer (args);
			} catch (Exception e) {
				Console.WriteLine ("error while serving application:");
				Console.WriteLine (e);
				return 1;
			}
			
			return 0;
		}
		
		public void RunServer (IList<string> args)
		{
			string port = null;
			string securePort = null;
			string certFile = null;
			string keyFile = null;
			string user = null;
			string assembly = null;
			string ipaddress = null;
			string browse = null;
			string docroot = null;
			
			var p = new OptionSet () {
				{ "p|port=", v => port = v },
				{ "P|secureport=", v => securePort = v },
				{ "c|certfile=", v => certFile = v },
				{ "k|keyfile=", v => keyFile = v },
				{ "u|user=", v => user = v },
				{ "a|assembly=", v=> assembly = v},
				{ "l|listen=", v => ipaddress = v },
				{ "b|browse=", v => browse = v },
				{ "d|docroot=", v => docroot = v}
			};
			args = p.Parse(args);

			ServerCommand cmd = new ServerCommand (Environment, args);
			
			if(assembly != null)
			{
				cmd.ApplicationAssembly = assembly;
			}
			
			if (port != null) {
				int pt;
				if (!Int32.TryParse (port, out pt))
					throw new ArgumentException ("Port value is not an integer.");
				if (pt <= 0)
					throw new ArgumentOutOfRangeException ("port", "Port must be a positive integer.");
				cmd.Port = pt;
			}
			
			if (securePort != null) {
				if (certFile == null)
					throw new ArgumentException ("Certificate file required for TLS.");
				if (keyFile == null)
					throw new ArgumentException ("Certificate private key required for TLS.");
				int pt;
				if (!Int32.TryParse (securePort, out pt))
					throw new ArgumentException ("Secure port value is not an integer.");
				if (pt <= 0)
					throw new ArgumentOutOfRangeException ("secureport", "Secure port must be a positive integer.");
				cmd.SecurePort = pt;
				cmd.CertificateFile = certFile;
				cmd.KeyFile = keyFile;
			}

			if (user != null)
				cmd.User = user;

			if (ipaddress != null)
				cmd.IPAddress = ipaddress;

			if (docroot != null)
				cmd.DocumentRoot = docroot;

			if (browse != null)
				cmd.Browse = browse;

			cmd.Run ();
		}
		
		private static int Docs (IList<string> args)
		{
			Driver d = new Driver ();
			
			try {
				d.RunDocs ();
			} catch (Exception e) {
				Console.WriteLine ("error while serving application:");
				Console.WriteLine (e);
				return 1;
			}
			
			return 0;
		}
		
		public void RunDocs ()
		{
			DocsCommand cmd = new DocsCommand (Environment);
			
			cmd.Run ();
		}

		private static int Build (IList<string> args)
		{
			Driver d = new Driver ();
			
			try {
				d.RunBuild ();
			} catch (Exception e) {
				Console.WriteLine ("error while building application:");
				Console.WriteLine (e);
				return 1;
			}
			
			return 0;
		}
		
		public void RunBuild ()
		{
			BuildCommand cmd = new BuildCommand (Environment);
			
			cmd.Run ();
		}

		private static int ShowEnvironment (IList<string> args)
		{
			Console.WriteLine ("libdir: '{0}'", Environment.LibDirectory);
			Console.WriteLine ("manosdir: '{0}'", Environment.ManosDirectory);
			Console.WriteLine ("workingdir: '{0}'", Environment.WorkingDirectory);
			Console.WriteLine ("datadir: '{0}'", Environment.DataDirectory);
			Console.WriteLine ("datadir: '{0}'", Environment.DocsDirectory);

			return 1;
		}

		private static void ShowHelp (OptionSet os)
		{
			Console.WriteLine ("manos usage is: manos [command] [options]");
			Console.WriteLine ();
			os.WriteOptionDescriptions (Console.Out);
		}
		
		private static int Run (string app, IList<string> args)
		{
			RunCommand cmd = new RunCommand ();
			return cmd.Run (app, args);
		}
	}
}
