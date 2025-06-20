using Microsoft.Extensions.Logging;
using RebelAlliance.TelnetDemo.Interfaces;
using System.Net.Sockets;

namespace RebelAlliance.TelnetDemo.Telnet;

public class TelnetClientFactory(IChatRoom chatRoom, ILogger<TelnetClient> logger, ITelnetFilter telnetFilter) : IClientFactory
{
    public IClient CreateClient(TcpClient tcpClient, short id)
    {
        return new TelnetClient(tcpClient, chatRoom, id, logger, telnetFilter);
    }
}
