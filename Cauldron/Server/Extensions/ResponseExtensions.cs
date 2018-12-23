using System;
using System.Text;
using System.Net;

namespace Cauldron
{
	static class ResponseExtensions
	{
		public static void Send(this HttpListenerResponse res, string str, Encoding encoding)
		{
			var data = encoding.GetBytes(str);
			res.OutputStream.Write(data, 0, data.Length);
			res.OutputStream.Flush();
			res.OutputStream.Close();
		}

		public static void End(this HttpListenerResponse res, int statusCode = 0)
		{
			if (statusCode != 0)
				res.StatusCode = statusCode;

			res.OutputStream.Flush();
			res.OutputStream.Close();
		}

		public static void WriteString(this HttpListenerResponse res, string str, Encoding encoding)
		{
			var data = encoding.GetBytes(str);
			res.OutputStream.Write(data, 0, data.Length);
		}
	}
}
