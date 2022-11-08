using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alibi
{
    internal class Webhookhandler
    {
        public static HttpClient _CLIENT = new HttpClient();
        public static string webhookURL = "";
        public static void send(string content)
        {

            Dictionary<string, string> _CONTENT = new Dictionary<string, string>()
            {
                {"content", content},
                {"username", "Hah!"},
            };
            _CLIENT.PostAsync(webhookURL, new FormUrlEncodedContent(_CONTENT));
        }

    }
}
