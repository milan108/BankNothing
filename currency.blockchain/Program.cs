using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;


namespace currency.blockchain
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            var chain = new Blockchain();
           // var server = new WebServer(chain);
            System.Console.Read();
        }
    }
}
