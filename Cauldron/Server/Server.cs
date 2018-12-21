using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

namespace Cauldron
{
	class Server
	{
		public delegate void RouteCallback(ClientRequest request, HttpListenerResponse response);

		struct Route
		{
            public string Url
            {
                get
                {
                    return string.Join("/", this.UrlParts);
                }
            }

            public string[] UrlParts;
			public bool Wildcard;
			public RouteCallback Callback;
		}

		HttpListener _listener;
		List<Route> _routes;

		public Server(int port)
		{
			_listener = new HttpListener();
			_listener.Prefixes.Add($"http://localhost:{port}/");
			_routes = new List<Route>();
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
							var route = CheckUrl(ctx);

							if (route != null)
								route.Value.Callback(new ClientRequest()
                                {
                                    Request = ctx.Request,
                                    RelativePath = ctx.Request.Url.AbsolutePath.Replace(route.Value.Url.Remove(route.Value.Url.IndexOf('+'), 1), "")
                                }, ctx.Response);
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

		private Route? CheckUrl(HttpListenerContext ctx)
		{
			var path = ctx.Request.Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var route in _routes)
			{
				var match = CheckRoute(route, path);

				if (match && (path.Length > route.UrlParts.Length) && route.Wildcard)
					return route;
				else if (match && (path.Length == route.UrlParts.Length))
					return route;
			}

			return null;
		}

		private bool CheckRoute(Route route, string[] path)
		{
			for (int i = 0; i < route.UrlParts.Length; i++)
			{
				if (path.Length < i)
					return false;

				if (path[i] != route.UrlParts[i])
					return false;
			}

			return true;
		}

		public void AddRoute(string path, RouteCallback callback)
		{
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));

			var parts = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			var wild = (parts.Length == 0) ? false : parts.Last().StartsWith("+");

			if (wild)
				parts = parts.Take(parts.Length - 1).ToArray();

			_routes.Add(new Route
			{
				UrlParts = parts,
				Callback = callback,
				Wildcard = wild
			});
		}
		
		public void Stop()
		{
			_listener.Stop();
			_listener.Close();
		}
	}
}
