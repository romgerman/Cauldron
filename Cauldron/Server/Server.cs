using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;

namespace Cauldron
{
	class Server
	{
		public delegate void RouteCallback(HttpListenerRequest request, HttpListenerResponse response);

        public Router Router => _router;

		HttpListener _listener;
        Router _router;

		public Server(int port)
		{
			_listener = new HttpListener();
			_listener.Prefixes.Add($"http://localhost:{port}/");
            _router = new Router();
		}

		public void Start()
		{
			_listener.Start();
			
			Console.WriteLine("Web server is running");

			while(_listener.IsListening)
			{
				ThreadPool.QueueUserWorkItem((obj) =>
				{
					var ctx = obj as HttpListenerContext;

					try
					{
						Console.WriteLine($"{ctx.Request.HttpMethod} {ctx.Request.Url.AbsolutePath}");
					
						var route = _router.CheckUrl(ctx);

						if (route != null)
							route.Callback(ctx.Request, ctx.Response, route);
						else
							ctx.Response.End(404);
					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
						ctx.Response.End();
					}

				}, _listener.GetContext());
			}
		}
		
		public void Stop()
		{
			_listener.Stop();
			_listener.Close();
		}
	}
}
