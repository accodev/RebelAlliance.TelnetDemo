using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using RebelAlliance.TelnetDemo.Interfaces;
using RebelAlliance.TelnetDemo.Telnet;
using System.Net.Sockets;
using Moq;

namespace RebelAlliance.TelnetDemo.UnitTests.Telnet;

[TestSubject(typeof(TelnetClientFactory))]
public class TelnetClientFactoryTest
{
    [Fact]
    public void CreateClient_ReturnsTelnetClient_WithCorrectDependencies()
    {
        // Arrange
        var chatRoom = new Mock<IChatRoom>().Object;
        var logger = new Mock<ILogger<TelnetClient>>().Object;
        var telnetFilter = new Mock<ITelnetFilter>().Object;
        var factory = new TelnetClientFactory(chatRoom, logger, telnetFilter);

        var tcpClient = new TcpClient();
        short id = 42;

        // Act
        var client = factory.CreateClient(tcpClient, id);

        // Assert
        Assert.NotNull(client);
        Assert.IsType<TelnetClient>(client);

        // Cleanup
        tcpClient.Dispose();
    }

    [Fact]
    public void CreateClient_ReturnsDifferentInstances_ForDifferentCalls()
    {
        // Arrange
        var chatRoom = new Mock<IChatRoom>().Object;
        var logger = new Mock<ILogger<TelnetClient>>().Object;
        var telnetFilter = new Mock<ITelnetFilter>().Object;
        var factory = new TelnetClientFactory(chatRoom, logger, telnetFilter);

        var tcpClient1 = new TcpClient();
        var tcpClient2 = new TcpClient();

        // Act
        var client1 = factory.CreateClient(tcpClient1, 1);
        var client2 = factory.CreateClient(tcpClient2, 2);

        // Assert
        Assert.NotSame(client1, client2);

        // Cleanup
        tcpClient1.Dispose();
        tcpClient2.Dispose();
    }
}