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
		private List<string> _absolutePaths;
	
		string _path;
		bool _relative;

		public StaticContentModule(string path) // TODO: support array
		{
			_path = path;
			_relative = !Path.IsPathRooted(path);
		}

		public override void OnResponse(HttpListenerRequest req, HttpListenerResponse res, Router.Route route)
		{
			base.OnResponse(req, res, route);
			
			var path = (_relative ? AppDomain.CurrentDomain.BaseDirectory : "") + _path + HttpUtility.UrlDecode(route.RelativePath(req.Url.AbsolutePath).Replace('/', Path.DirectorySeparatorChar));
			var attrs = File.GetAttributes(path);

			if (!attrs.HasFlag(FileAttributes.Directory))
			{
				var extension = Path.GetExtension(path);
				string mime = null;

				MIME.ExtensionToMIME.TryGetValue(extension, out mime);

				res.ContentType = mime;
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
							{
								writer.Write(buff, 0, buff.Length);
							}
						}
					}

					res.End();
				});
			}
			else if (attrs.HasFlag(FileAttributes.Directory))
			{
				res.StatusCode = 200;
				res.ContentEncoding = Encoding.UTF8;

				Console.WriteLine("hey");
				string[] files = Directory.GetFiles(path);
				
				foreach(var file in files)
				{
					res.WriteString($"{Path.GetFileName(file)}\n", Encoding.UTF8);
				}

				res.End();
			}
			else
			{
				res.End(404);
				return;
			}
		}
	}
}
