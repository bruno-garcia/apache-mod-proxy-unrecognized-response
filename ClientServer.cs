using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        switch (args[0])
        {
            case "-s":
                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls(args[1])
                    .UseStartup<Program>()
                    .Build();
                host.Run();
                break;

            case "-c":
                var client = new HttpClient();
                while (true)
                {
                    Console.WriteLine(client.GetAsync(args[1]).GetAwaiter().GetResult().StatusCode);
                    Thread.Sleep(5000);
                }
        }
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole();

        app.Run(context =>
        {
            context.Response.StatusCode = 204;
            return Task.CompletedTask;
        });
    }
}

