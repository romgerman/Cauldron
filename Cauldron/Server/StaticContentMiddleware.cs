using System;
using System.IO;
using System.Net;
using System.Text;

namespace Cauldron
{
	class StaticContentMiddleware : Middleware
	{
		string url;
		string path;

		public StaticContentMiddleware(string url, string path)
		{
			this.url = url;
			this.path = path;
		}

		public override void OnResponse(HttpListenerRequest req, HttpListenerResponse res)
		{
			base.OnResponse(req, res);

			var start = req.Url.AbsolutePath.Replace(url, "").Replace('/', Path.DirectorySeparatorChar);
			var path = AppDomain.CurrentDomain.BaseDirectory + start;

			if (!File.Exists(path))
			{
				res.StatusCode = 404;
			}

			var attrs = File.GetAttributes(path);

			if (!attrs.HasFlag(FileAttributes.Directory))
			{
				var extension = Path.GetExtension(path);
				string mime = null;

				MIME.ExtensionToMIME.TryGetValue(extension, out mime);

				res.ContentType = mime;

				var buff = new byte[1024];

				using (var fileStream = File.OpenRead(path))
				{
					using (var writer = new BinaryWriter(res.OutputStream))
					{
						while (fileStream.Read(buff, 0, buff.Length) > 0)
						{
							writer.Write(buff, 0, buff.Length);
						}
					}
				}
			}
			else
			{
				res.StatusCode = 404;
				res.Send("not supported", Encoding.UTF8);
			}
		}
	}
}
