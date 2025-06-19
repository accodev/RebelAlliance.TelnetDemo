using Microsoft.Extensions.Logging;
using RebelAlliance.TelnetDemo.Interfaces;
using System.Net.Sockets;

namespace RebelAlliance.TelnetDemo.Telnet;

internal class TelnetServer(string host, int port, IClientFactory clientFactory, ILogger<TelnetServer> logger) : IServer
{
    private bool _isRunning = true;
    private readonly TcpListener _listener = new(System.Net.IPAddress.Parse(host), port);

    public async Task Execute(CancellationToken cancellationToken)
    {
        _listener.Start();
        logger.LogInformation("Telnet server started on {Host}:{Port}", host, port);

        await AcceptClientsAsync(cancellationToken);
    }

    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        while (_isRunning)
        {
            try
            {
                var tcpClient = await _listener.AcceptTcpClientAsync(cancellationToken);

                var id = (short)Random.Shared.Next(0, short.MaxValue);
                logger.LogInformation("Client {Id} connected.", id);
                var client = clientFactory.CreateClient(tcpClient, id);
                _ = client.Handle(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error accepting client"); 
            }
            finally
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Stop();
                }
            }
        }
    }

    public void Stop()
    {
        _isRunning = false;
        _listener.Stop();
        logger.LogInformation("Server stopped.");
    }
}