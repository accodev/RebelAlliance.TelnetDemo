using Microsoft.Extensions.Logging;
using RebelAlliance.TelnetDemo.Interfaces;
using System.Net.Sockets;

namespace RebelAlliance.TelnetDemo.Telnet;

internal class ChatClientFactory(IChatRoom chatRoom, ILogger<TelnetClient> logger) : IClientFactory
{
    public IClient CreateClient(TcpClient tcpClient, short id)
    {
        return new TelnetClient(tcpClient, chatRoom, id, logger);
    }
}
