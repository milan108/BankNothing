using Newtonsoft.Json;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;

namespace currency.blockchain
{
    public class WebServer
    {
        public WebServer(Blockchain chain)
        {
            var settings = ConfigurationManager.AppSettings;
            string host = settings["host"]?.Length > 1 ? settings["host"] : "localhost";
            string port = settings["port"]?.Length > 1 ? settings["port"] : "12345";


        }
    }
}