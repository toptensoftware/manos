Manos is an easy to use, easy to test, high performance web application framework that stays out of your way and makes your life ridiculously simple.

The Main Features of Manos:

 - **Manos creates stand alone web applications** that don't rely on Apache, IIS or any other web server, the server is bundled into the .exe file with the rest of your application.  This allows for easy deployment, easy upgrades and absolutely no configuration. 

 - **High performance and scalable.**  Modeled after tornadoweb, the technology that powers friend feed, Manos is capable of thousands of simultaneous connections, ideal for applications that create persistent connections with the server.

 - **An easy to use, high performance template engine** Manos comes bundled with a designer friendly templating language. Just because the template engine is easy to use, doesn't mean its slow though.  Manos's template engine precompiles your templates to IL so there are no startup costs and templates are rendered lightning fast.

 - **Testing built in** from the beginning making things testable was a primary concern on Manos.  This means you can easily Mock just about any object in the Manos stack, making testing your application dead simple.


For installation instructions please read the installation guide in the docs directory.


## About this Fork of Manos

This fork of Manos is intended to simplify development under Visual Studio/Windows

Maintained by Brad Robinson (@toptensoftware - http://toptensoftware.com) 

### Build Improvements

* Fixed build issues under Windows (a number of uninitialized variable errors)
* Includes a new dependencies folder that includes Mono.Posix, Mono.C5 and Mono.CSharp so as to not have to install Mono.
* Updated build procedure copies the `data` and `docs` folder to the build output directory
* Updated ManosTool looks for data directory in same folder as exe allowing running ManosTool directly from build output directory.
* Should be able to clone this repo and build directly in Visual Studio 2010 with no additional dependencies.

### Bug Fixes

* Fixed TimerWatchers not working under Windows, including not firing and single shot timers firing repeatedly.

### StandAlone EXE's

A new assembly - Manos.Ext.dll - includes InprocServer class.

To create a web app:

1. In visual studio create a new Console Application
2. Add references to Manos and Manos.Ext
3. Create you main Manos app class deriving from ManosApp
4. Implement Main by calling Manos.Ext.InprocServer.Run 

eg:

	namespace Shorty {

		public class Shorty : ManosApp {

			public Shorty ()
			{
				Route ("/Content/", new StaticContentModule ());
			}

			public static int Main(string[] args)
			{
				return Manos.Ext.InprocServer.Run(args);
			}
		}
	}

This will run a standalone Manos server.  Most settings are read from app.config but all are optional:

	<?xml version="1.0" encoding="utf-8" ?>
	<configuration>
		<appSettings>
			<add key="Manos.Port" value="8080"/>
			<add key="Manos.SecurePost" value=""/>
			<add key="Manos.User" value=""/>
			<add key="Manos.IPAddress" value=""/>
			<add key="Manos.CertificateFile" value=""/>
			<add key="Manos.KeyFile" value=""/-->
			<add key="Manos.DocumentRoot" value=""/>
		</appSettings>
	</configuration>

There are also some command line arguments:

	--documentroot:<docroot>		- specifies the document root, uses app.config or current directory if not specified.
	--browse						- launches default browser at http://localhost:<port> after starting server
	--browse:<url>					- lanuches default browser at specified URL after starting server

Because VisualStudio runs the app in with the bin\[Debug\Release] folder as the current working directory, you need
to tell your app where to find your content (document root).  You can do this by specifying the following command line
argument in Project Settings -> Debug -> Commandline Arguments: 

	--documentroot:..\..

