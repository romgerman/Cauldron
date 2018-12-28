using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron
{
	static class MIME
	{
		public static string OctetStream = "application/octet-stream";

		public static Dictionary<string, string> MIMEtoExtension = new Dictionary<string, string>()
		{
			{ "application/json", "json" },
			{ "application/javascript", "js" },
			{ "application/ogg", "ogg" },
			{ "application/pdf", "pdf" },
			{ "application/postscript", "ps" },
			{ "application/font-woff", "woff" },
			{ "application/xhtml+xml", "xhtml" },
			{ "application/zip", "zip" },
			{ "application/gzip", "gzip" },
			{ "application/x-bittorrent", "torrent" },
			{ "application/xml", "xml" },

			{ "audio/mp4", "mp4" },
			{ "audio/aac", "aac" },
			{ "audio/mpeg", "mp3" },
			{ "audio/ogg", "ogg" },
			{ "audio/vorbis", "ogg" },
			{ "audio/x-ms-wma", "wma" },
			{ "audio/x-ms-wax", "wma" },
			{ "audio/vnd.rn-realaudio", "rm" },
			{ "audio/vnd.wave", "wav" },
			{ "audio/webm", "webm" },

			{ "image/gif", "gif" },
			{ "image/jpeg", "jpg" },
			{ "image/pjpeg", "jpeg" },
			{ "image/png", "png" },
			{ "image/svg+xml", "svg" },
			{ "image/tiff", "tif" },
			{ "image/vnd.microsoft.icon", "ico" },
			{ "image/vnd.wap.wbmp", "wbmp" },
			{ "image/webp", "webp" },

			{ "text/css", "css" },
			{ "text/csv", "csv" },
			{ "text/html", "html" },
			{ "text/plain", "txt" },
			{ "text/php", "php" },
			{ "text/xml", "xml" },
			{ "text/markdown", "md" },

			{ "video/mpeg", "mpeg" },
			{ "video/mp4", "mp4" },
			{ "video/ogg", "ogg" },
			{ "video/quicktime", "mov" },
			{ "video/webm", "webm" },
			{ "video/x-ms-wmv", "wmv" },
			{ "video/x-flv", "flv" },
			{ "video/3gpp", "3gp" },
			{ "video/3gpp2", "3gpp2" }
		};

		public static Dictionary<string, string> ExtensionToMIME = MIMEtoExtension.Reverse().IEnumerableToDictionary();
	}
}
