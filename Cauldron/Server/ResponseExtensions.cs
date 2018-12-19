using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Cauldron
{
	static class ResponseExtensions
	{
		public static void Send(this HttpListenerResponse res, string str, Encoding encoding)
		{
			var data = encoding.GetBytes(str);
			res.OutputStream.Write(data, 0, data.Length);
		}
	}
}
