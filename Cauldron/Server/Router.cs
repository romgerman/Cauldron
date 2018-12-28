using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Cauldron
{
	class Router
	{
		public delegate void RouteCallback(HttpListenerRequest request, HttpListenerResponse response, Route route);

		public enum Method
		{
			GET, POST, PUT, DELETE, HEAD, OPTIONS
		}

		public class Route
		{
			public Method HttpMethod;
			public string Path;
			public Regex Regex;
			public bool Wildcard;
			public RouteCallback Callback;

			public string[] UrlParts;			

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
		
		public Route MatchRequest(HttpListenerContext ctx)
		{
			var route = CheckUrl(ctx);

			if (route == null)
				return null;

			if (!route.HttpMethod.ToString().Equals(ctx.Request.HttpMethod))
				return null;

			return route;
		}

		private bool PathIsFile(string path) => (path.LastIndexOf('.') > 0);

		private Route CheckUrl(HttpListenerContext ctx)
		{
			var path = ctx.Request.Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var route in _routes)
			{
				var match = CheckPath(route, path);

				if (route.Wildcard && !PathIsFile(ctx.Request.Url.AbsolutePath) && !ctx.Request.Url.AbsolutePath.EndsWith("/"))
					continue;

				else if (match && (path.Length > route.UrlParts.Length) && route.Wildcard)
					return route;
				else if (match && (path.Length == route.UrlParts.Length))
					return route;
			}

			return null;
		}

		private bool CheckPath(Route route, string[] path)
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

		public void AddRoute(string path, RouteCallback callback, Method method = Method.GET)
		{
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));

			if (path != "/" &&
				path.LastIndexOf('/') != (path.Length - 1) &&
				(path.LastIndexOf('/') != (path.Length - 2) && path.EndsWith("/")))
				throw new ArgumentException("Path should end with trailing slash!", nameof(path));

			var parts = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			var end = parts.LastOrDefault();
			var isWild = end != null && ((parts.Length == 0) ? false : end.EndsWith("+"));
			var isRegex = end != null && (end[0] == '[' && (end[end.Length - 1] == ']' || end[end.Length - 2] == ']'));

			if (isRegex && isWild)
				end = end.Remove(end.Length - 1, 1);

			Regex regex = null;

			if (isRegex)
				regex = new Regex(end, RegexOptions.Compiled);

			if (isWild) // Remove plus sign
				parts = parts.Take(parts.Length - 1).ToArray();

			_routes.Add(new Route
			{
				UrlParts = parts,
				Callback = callback,
				Wildcard = isWild,
				Path = path,
				HttpMethod = method,
				Regex = regex
			});
		}

		public void Get(string path, RouteCallback callback) => AddRoute(path, callback, Method.GET);
		public void Post(string path, RouteCallback callback) => AddRoute(path, callback, Method.POST);
	}
}
