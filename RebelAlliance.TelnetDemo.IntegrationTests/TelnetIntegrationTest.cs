using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using RebelAlliance.TelnetDemo.Chat;
using RebelAlliance.TelnetDemo.Interfaces;
using RebelAlliance.TelnetDemo.Telnet;

namespace RebelAlliance.TelnetDemo.IntegrationTests;

public class TelnetIntegrationTest
{
    [Fact]
    public async Task TelnetServer_And_TelnetClient_Integration_Works()
    {
        // Arrange
        var chatRoom = new ChatRoom();
        var telnetFilter = new TelnetFilter();
        var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug().SetMinimumLevel(LogLevel.Trace));
        var serverLogger = loggerFactory.CreateLogger<TelnetServer>();
        var clientLogger = loggerFactory.CreateLogger<TelnetClient>();
        var randomProvider = new Mock<IRandomProvider>();

        const int clientId = 10;

        randomProvider
            .Setup(x => x.Provide(It.IsAny<short>(), It.IsAny<short>()))
            .Returns(clientId);

        // Use a random available port
        var port = GetAvailablePort();
        var host = "127.0.0.1";

        // Use a real client factory
        var clientFactory = new TelnetClientFactory(chatRoom, clientLogger, telnetFilter);

        var server = new TelnetServer(host, port, clientFactory, serverLogger, randomProvider.Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        var serverTask = server.Execute(cts.Token);

        // Wait for the server to start
        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(host, port, cts.Token);

        var stream = tcpClient.GetStream();
        var buffer = new byte[1024];

        // Read welcome message
        var welcomeLen = await stream.ReadAsync(buffer, cts.Token);
        var welcomeMsg = Encoding.ASCII.GetString(buffer, 0, welcomeLen);
        Assert.Contains($"Welcome Client{clientId}", welcomeMsg);

        // Send a chat message
        var message = "Hello, world!\r\n";
        var messageBytes = Encoding.ASCII.GetBytes(message);
        await stream.WriteAsync(messageBytes, 0, messageBytes.Length, cts.Token);

        // Clean up
        cts.Cancel();
        server.Stop();
        await Task.WhenAny(serverTask, Task.Delay(1000));
    }

    private static int GetAvailablePort()
    {
        var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
