using System;
using System.Net;
using System.Threading;

namespace Cauldron
{
	class Server
	{
		public delegate void RouteCallback(HttpListenerRequest request, HttpListenerResponse response);

        public Router Router { get; private set; }

		HttpListener _listener;

		public Server(int port)
		{
			_listener = new HttpListener();
			_listener.Prefixes.Add($"http://localhost:{port}/");
            Router = new Router();
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
						var route = Router.MatchRequest(ctx);

						if (route != null)
							route.Callback(ctx.Request, ctx.Response, route);
						else
							ctx.Response.End(404);

						Console.WriteLine($"{ctx.Request.HttpMethod} {ctx.Request.Url.AbsolutePath} - {ctx.Response.StatusCode}");
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
