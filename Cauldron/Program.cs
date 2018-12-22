using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Json = Newtonsoft.Json.Linq;

namespace Cauldron
{
	class Program
	{
		static string defaultJson = @"
			{
				""modules"": [ ""static"" ],
				""hosts"": {
					""port"": 11000,
					""paths"": {
						""/"": {
							""echo"": ""Hello world!""
						},
						""/files/+"": {
							""modules"": {
								""static"": {
									""paths"": [ ""content"" ]
								}
							}
						}
					}
				}
			}
		";

		static void Main(string[] args)
		{
			//var config = new Config("./config.json", Json.JObject.Parse(defaultJson));

			var server = new Server(11000);

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

			server.Router.AddRoute("/files/+", (req, res, r) => new StaticContentMiddleware("/static/").OnResponse(req, res, r));
			
			server.Start();
			Console.WriteLine("Press any key to shutdown");
			Console.ReadLine();
			server.Stop();
		}

	}
	
}
