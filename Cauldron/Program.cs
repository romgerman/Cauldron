using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cauldron
{
    class Program
    {
        static void Main(string[] args)
        {
			var config = new Config("./config.json", new Newtonsoft.Json.Linq.JObject()
			{
				{ "port", 11000 },
				{ "static", new Newtonsoft.Json.Linq.JArray() { "./" } }
			});

			var server = new Server(config.Get<int>("port"));

			server.AddRoute("/", (req, res) =>
			{
				res.StatusCode = 200;
				res.Send("Hello damn girl", Encoding.UTF8);
			});

			var folders = config.Get<string[]>("static");

			server.AddRoute("/content/+", (req, res) => new StaticContentMiddleware("/content", "/static/").OnResponse(req.Request, res));
			
			server.Start();
			Console.WriteLine("Press any key to shutdown");
			Console.ReadLine();
			server.Stop();
        }

    }
	
}
