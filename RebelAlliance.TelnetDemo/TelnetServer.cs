using System.Net.Sockets;

namespace RebelAlliance.TelnetDemo;

internal class TelnetServer(string host, int port)
{
    private bool _isRunning;

    private readonly TcpListener _listener = new(System.Net.IPAddress.Parse(host), port);
    private readonly ChatRoom _chatRoom = new();

    public async Task Start()
    {
        _listener.Start();
        _isRunning = true;
        Console.WriteLine("Server started. Waiting for clients...");

        await AcceptClientsAsync();
    }

    private async Task AcceptClientsAsync()
    {
        while (_isRunning)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");

                // Handle client in separate task
                var telnetClient = new TelnetClient(client, _chatRoom);
                _ = telnetClient.Handle();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }
    }

    public void Stop()
    {
        _isRunning = false;
        _listener.Stop();
        Console.WriteLine("Server stopped.");
    }
}