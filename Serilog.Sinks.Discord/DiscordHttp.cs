using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace Serilog.Sinks.Discord
{
    public class DiscordHttp
    {
        public static byte[] Post(string url, NameValueCollection pairs)
        {
            using (WebClient client = new WebClient())
                return client.UploadValues(url, pairs);
        }
    }
}
