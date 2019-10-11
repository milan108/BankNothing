using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;


namespace currency.blockchain
{
    class Program
    {
        public static void Main(string[] args)
        {
            using (var host = WebHost.Start(router => router
                            .MapGet("hello/{name}", (req, res, data) =>
                                res.WriteAsync($"Hello, {data.Values["name"]}!"))
                            .MapGet("buenosdias/{name}", (req, res, data) =>
                                res.WriteAsync($"Buenos dias, {data.Values["name"]}!"))
                            .MapGet("throw/{message?}", (req, res, data) =>
                                throw new Exception((string)data.Values["message"] ?? "Uh oh!"))
                            .MapGet("{greeting}/{name}", (req, res, data) =>
                                res.WriteAsync($"{data.Values["greeting"]}, {data.Values["name"]}!"))
                            .MapGet("", (req, res, data) => res.WriteAsync("Hello, World!"))))
            {
                Console.WriteLine("Use Ctrl-C to shutdown the host...");
                host.WaitForShutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddCommandLine(args)
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:4000")
                .UseConfiguration(config)
                .Configure(app =>
                {
                    app.Run(context =>
                        context.Response.WriteAsync("Hello, World!"));
                });
        }
    }
}
