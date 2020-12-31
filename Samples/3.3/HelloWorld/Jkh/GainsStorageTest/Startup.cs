namespace GainsStorageTest
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    internal class Startup : IHostedService
    {
        private readonly ILogger<Startup> logger;
        private readonly IConfiguration configuration;

        public Startup(ILogger<Startup> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Started Service");

#if DEBUG            
            logger.LogDebug("debug logger");
            logger.LogTrace("trace logger");
            logger.LogWarning("warning logger");
            logger.LogError(new ArgumentException("argument is wrong"), "this is a test");
            logger.LogCritical("critical logger");
#endif
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Ended Service");
            return Task.CompletedTask;
        }
    }
}