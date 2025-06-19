using System.Net.Sockets;

namespace RebelAlliance.TelnetDemo.Interfaces;

internal interface IClientFactory
{
    IClient CreateClient(TcpClient tcpClient, short id);
}
