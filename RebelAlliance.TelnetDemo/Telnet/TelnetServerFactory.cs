using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RebelAlliance.TelnetDemo.Interfaces;

namespace RebelAlliance.TelnetDemo.Telnet;

public class TelnetServerFactory(IClientFactory clientFactory, 
    IOptions<ServerSettings> options, 
    ILogger<TelnetServer> logger,
    IRandomProvider randomProvider) : IServerFactory
{
    public IServer CreateServer()
    {
        var settings = options.Value;
        return new TelnetServer(settings.Host, settings.Port, clientFactory, logger, randomProvider);
    }
}
