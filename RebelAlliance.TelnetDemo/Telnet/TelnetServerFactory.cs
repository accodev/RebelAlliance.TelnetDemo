using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RebelAlliance.TelnetDemo.Interfaces;

namespace RebelAlliance.TelnetDemo.Telnet;

internal class TelnetServerFactory(IClientFactory clientFactory, 
    IOptions<ServerSettings> options, 
    ILogger<TelnetServer> logger) : IServerFactory
{
    public IServer CreateServer()
    {
        var _settings = options.Value;
        return new TelnetServer(_settings.Host, _settings.Port, clientFactory, logger);
    }
}
