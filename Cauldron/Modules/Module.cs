using System;
using System.Net;

namespace Cauldron
{
	class Module
	{
		public virtual void OnResponse(HttpListenerRequest req, HttpListenerResponse res, Router.Route route)
		{
		}
	}
}
