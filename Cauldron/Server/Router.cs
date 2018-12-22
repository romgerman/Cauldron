using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Cauldron
{
	class Router
	{
		public delegate void RouteCallback(HttpListenerRequest request, HttpListenerResponse response, Route route);

		public class Route
		{
			public string Path;
			public string[] UrlParts;
			public bool Wildcard;
			public RouteCallback Callback;

			public string RelativePath(string url)
			{			
				return url.Replace(this.Path.Remove(this.Path.IndexOf('+'), 1), "");
			}
		}

		private List<Route> _routes;

		public Router()
		{
			_routes = new List<Route>();
		}

		private bool PathIsFile(string path)
		{
			return path.LastIndexOf('.') > 0;
		}

		public Route CheckUrl(HttpListenerContext ctx)
		{		
			var path = ctx.Request.Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var route in _routes)
			{
				var match = CheckRoute(route, path);

				if (route.Wildcard && !PathIsFile(ctx.Request.Url.AbsolutePath) && !ctx.Request.Url.AbsolutePath.EndsWith("/"))
					continue;

				else if (match && (path.Length > route.UrlParts.Length) && route.Wildcard)
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

		// TODO: regex 
		public void AddRoute(string path, RouteCallback callback)
		{
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));

			if (path != "/" &&
				path.LastIndexOf('/') != (path.Length - 1) &&
				(path.LastIndexOf('/') != (path.Length - 2) && path.EndsWith("/")))
				throw new ArgumentException("Path should end with trailing slash!", nameof(path));

			var parts = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			var wild = (parts.Length == 0) ? false : parts.Last().StartsWith("+");

			if (wild) // remove plus sign
				parts = parts.Take(parts.Length - 1).ToArray();

			_routes.Add(new Route
			{
				UrlParts = parts,
				Callback = callback,
				Wildcard = wild,
				Path = path
			});
		}
	}
}
