using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RebelAlliance.TelnetDemo.Interfaces;

namespace RebelAlliance.TelnetDemo;

internal class HostedService(IServerFactory serverFactory, ILogger<HostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Executing");

        var server =
            serverFactory.CreateServer();

        await server.Execute(cancellationToken);

        logger.LogInformation("Finished executing. Exiting.");
    }
}
