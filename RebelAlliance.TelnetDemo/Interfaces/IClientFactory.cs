using System.Net.Sockets;

namespace RebelAlliance.TelnetDemo.Interfaces;

public interface IClientFactory
{
    IClient CreateClient(TcpClient tcpClient, short id);
}
