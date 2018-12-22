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

			ThreadPool.QueueUserWorkItem((o) =>
			{
				Console.WriteLine("Web server is running");

				while(_listener.IsListening)
				{
					ThreadPool.QueueUserWorkItem((obj) =>
					{
						var ctx = obj as HttpListenerContext;

						try
						{
							var route = _router.CheckUrl(ctx);

							if (route != null)
								route.Callback(ctx.Request, ctx.Response, route);
							else
								ctx.Response.StatusCode = 404;
						}
						catch (Exception e)
						{
							Console.WriteLine(e.ToString());
						}
						finally
						{
							ctx.Response.OutputStream.Close();
						}

					}, _listener.GetContext());
				}
			});
		}
		
		public void Stop()
		{
			_listener.Stop();
			_listener.Close();
		}
	}
}
