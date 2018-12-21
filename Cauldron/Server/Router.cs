using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron
{
    class Router
    {
        public delegate void RouteCallback(HttpListenerRequest request, HttpListenerResponse response);

        class Route
        {
            public string Url;
            public string[] UrlParts;
            public bool Wildcard;
            public RouteCallback Callback;
        }

        private List<Route> _routes;

        public Router()
        {
            _routes = new List<Route>();
        }

        private Route CheckUrl(HttpListenerContext ctx)
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
    }
}
