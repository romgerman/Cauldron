using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Cauldron
{
    class ClientRequest
    {
        public string RelativePath { get; set; }

        public HttpListenerRequest Request;
    }
}
