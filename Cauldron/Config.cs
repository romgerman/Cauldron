using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Cauldron
{
	class Config
	{
		JObject _data;

		public Config(string path, JObject defaultValues)
		{
			if (!File.Exists(path))
			{
				using (var stream = File.Create(path))
				{
					var defaultText = Encoding.UTF8.GetBytes(defaultValues.ToString());
					stream.Write(defaultText, 0, defaultText.Length);
				}
			}

			_data = JObject.Parse(File.ReadAllText(path, Encoding.UTF8));
		}

		public T Get<T>(string path)
		{
			string[] keys = path.Split('/');

			return GetInside<T>(_data, keys);
		}

		private T GetInside<T>(JObject obj, string[] keys)
		{
			var key = keys[0];

			if (obj[key].Type == JTokenType.Object)
			{
				return GetInside<T>(obj, keys.Skip(1).ToArray());
			}

			return obj[key].ToObject<T>();
		}
	}
}
