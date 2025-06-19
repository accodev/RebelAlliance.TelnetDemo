using Microsoft.Extensions.Logging;
using RebelAlliance.TelnetDemo.Interfaces;
using System.Net.Sockets;

namespace RebelAlliance.TelnetDemo.Chat;

internal class ChatClientFactory(IChatRoom chatRoom, ILogger<ChatClient> logger) : IClientFactory
{
    public IClient CreateClient(TcpClient tcpClient, short id)
    {
        return new ChatClient(tcpClient, chatRoom, id, logger);
    }
}
