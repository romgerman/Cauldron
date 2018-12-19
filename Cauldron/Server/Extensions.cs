using System;
using System.Collections.Generic;

namespace Cauldron
{
	static class Extensions
	{
		static public Dictionary<T, U> IEnumerableToDictionary<T, U>(this IEnumerable<KeyValuePair<T, U>> e)
		{
			var dict = new Dictionary<T, U>();

			foreach(var kvp in e)
			{
				dict.Add(kvp.Key, kvp.Value);
			}

			return dict;
		}
	}
}
