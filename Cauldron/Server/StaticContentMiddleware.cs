using System;
using System.IO;
using System.Net;
using System.Text;

namespace Cauldron
{
	class StaticContentMiddleware : Middleware
	{
		string _path;

		public StaticContentMiddleware(string path)
		{
			this._path = path;
		}

		public override void OnResponse(HttpListenerRequest req, HttpListenerResponse res, Router.Route route)
		{
			base.OnResponse(req, res, route);
			
			var path = AppDomain.CurrentDomain.BaseDirectory + _path + route.RelativePath(req.Url.AbsolutePath).Replace('/', Path.DirectorySeparatorChar);

			Console.WriteLine(path);

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
			else if (attrs.HasFlag(FileAttributes.Directory))
			{
				res.StatusCode = 200;

				Console.WriteLine("hey");
				string[] files = Directory.GetFiles(path);
				
				foreach(var file in files)
				{
					res.Send($"{Path.GetFileName(file)}\n", Encoding.UTF8);
				}
			}
			else
			{
				if (!File.Exists(path))
				{
					res.StatusCode = 404;
					return;
				}
			}
		}
	}
}
