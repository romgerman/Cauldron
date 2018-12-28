using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cauldron
{
	class StaticContentModule : Module
	{
		public override string Name => "static";

		List<string> _paths;

		public StaticContentModule(string[] paths)
		{
			_paths = new List<string>(paths.Length);

			foreach (var path in paths)
				_paths.Add((Path.IsPathRooted(path) ? AppDomain.CurrentDomain.BaseDirectory : "") + path);
		}

		public override void OnResponse(HttpListenerRequest req, HttpListenerResponse res, Router.Route route)
		{
			base.OnResponse(req, res, route);

			var filePath = HttpUtility.UrlDecode(route.RelativePath(req.Url.AbsolutePath).Replace('/', Path.DirectorySeparatorChar));
			string foundFile = null;

			foreach(var path in _paths)
			{
				var fullPath = path + filePath;

				if (File.Exists(fullPath) || Directory.Exists(fullPath))
				{
					foundFile = fullPath;
					break;
				}				
			}

			if (foundFile == null)
			{
				res.End(404);
				return;
			}

			var fileAttrs = File.GetAttributes(foundFile);

			if (!fileAttrs.HasFlag(FileAttributes.Directory))     // Send file
			{
				ServeFile(foundFile, res);
			}
			else if (fileAttrs.HasFlag(FileAttributes.Directory)) // Send directory
			{
				ServeFolder(foundFile, res);
			}
			else
			{
				res.End(404);
				return;
			}
		}

		private void ServeFile(string path, HttpListenerResponse res)
		{
			var extension = Path.GetExtension(path);

			string mime = null;
			MIME.ExtensionToMIME.TryGetValue(extension, out mime);

			res.ContentType = mime ?? MIME.OctetStream;
			res.StatusCode = 200;

			Console.WriteLine($"Sending file \"{path}\"");

			Task.Run(() =>
			{
				var buff = new byte[2048];

				using (var fileStream = File.OpenRead(path))
				{
					using (var writer = new BinaryWriter(res.OutputStream))
					{
						while (fileStream.Read(buff, 0, buff.Length) > 0)
							writer.Write(buff, 0, buff.Length);
					}
				}

				res.End();
			});
		}

		private void ServeFolder(string path, HttpListenerResponse res)
		{
			res.StatusCode = 200;
			res.ContentEncoding = Encoding.UTF8;

			Console.WriteLine("hey");
			string[] files = Directory.GetFiles(path);
			string[] folders = Directory.GetDirectories(path);

			foreach (var file in folders)
			{
				res.WriteString($"/{Path.GetFileName(file)}/\n", Encoding.Default);
			}

			foreach (var file in files)
			{
				res.WriteString($"/{Path.GetFileName(file)}\n", Encoding.Default);
			}

			res.End();
		}
	}
}
