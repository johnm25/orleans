namespace GainsStorageTest
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    internal class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = null;

            try
            {
                var configuration = CreateConfigurationBuilder(args);

                logger =
                    CreateLogger(configuration)
                    .ForContext<Program>();

                logger.Information("Started Host");

                var host = Host
                    .CreateDefaultBuilder(args: args)
                    .ConfigureServices((hostContext, service) =>
                    {
                        service.AddLogging();
                        service.AddHostedService<Startup>();
                    })
                    .UseSerilog()
                    .Build();

                host.Run();
            }
            catch (Exception e)
            {
                logger?.Fatal(e, "Host terminated unexpectedly");
                throw;
            }

            finally
            {
                logger?.Information("Ended Host");
                Log.CloseAndFlush();
            }
        }

        private static IConfiguration CreateConfigurationBuilder(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Production}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }

        private static ILogger CreateLogger(IConfiguration configuration)
        {
            var loggerConfig = new LoggerConfiguration();

            loggerConfig
                .ReadFrom.Configuration(configuration, sectionName: "Logging:Serilog");

            ///////////////////////////// PROGRAMMATIC //////////////////////////////
            //loggerConfig
            //    .WriteTo.Console();

            //// CONFIGURE BASIC LOGGING TO A FILE
            //// 1.  Enrich log Events with Thread information using the Serilog enricher.
            //// 2.  Write log files to local storage
            //loggerConfig
            //    .Enrich.WithThreadName()
            //    .Enrich.WithThreadId()
            //    .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "SerilogExtensions.log"));

            //// https://docs.datalust.co/docs/getting-started-with-docker
            //// docker run -d --name seq -e ACCEPT_EULA=Y -p 5333:80  -p 5341:5341 --restart unless-stopped -v d:/logs/seq:/data datalust/seq:latest
            //loggerConfig
            //    .WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341"
            ///////////////////////////// PROGRAMMATIC //////////////////////////////

            return Log.Logger = loggerConfig.CreateLogger();
        }
    }
}
