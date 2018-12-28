using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

using Json = Newtonsoft.Json.Linq;

namespace Cauldron
{
	class Program
	{
		static string defaultJson = @"
			{
				""port"": 11000,
				""paths"": {
					""/"": {
						""echo"": ""Hello world!"",
						""status"": 200
					},
					""/files/+"": {
						""static"": {
							""paths"": [ ""content"" ]
						}
					}
				}
			}
		";

		static void Main(string[] args)
		{
			var config = new Config("./config.json", Json.JObject.Parse(defaultJson)); // TODO
			var server = new Server(config.Get<int>("port"));
			var workingThread = new Thread(() => server.Start());

			server.Router.AddRoute("/", (req, res, route) =>
			{
				res.StatusCode = 200;
				res.Send("Hello damn girl", Encoding.UTF8);
			});

			server.Router.AddRoute("/test/+", (req, res, r) =>
			{
				res.StatusCode = 200;
				res.Send($"Route path: {r.Path}\n", Encoding.UTF8);
				res.Send($"Absolute path: {req.Url.AbsolutePath}\n", Encoding.UTF8);
				res.Send($"Relative path: {r.RelativePath(req.Url.AbsolutePath)}", Encoding.UTF8);
			});

			server.Router.Get("/files/+", (req, res, r) => new StaticContentModule(new string[] { "static/" }).OnResponse(req, res, r));

			workingThread.Start();
			Console.WriteLine("Press any key to shutdown");
			Console.ReadLine();
			server.Stop();
		}

	}
	
}
