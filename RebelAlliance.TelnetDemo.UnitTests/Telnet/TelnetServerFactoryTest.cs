using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RebelAlliance.TelnetDemo.Interfaces;
using RebelAlliance.TelnetDemo.Telnet;

namespace RebelAlliance.TelnetDemo.UnitTests.Telnet;

[TestSubject(typeof(TelnetServerFactory))]
public class TelnetServerFactoryTest
{
    [Fact]
    public void CreateServer_ReturnsTelnetServer_WithCorrectSettings()
    {
        // Arrange
        var clientFactoryMock = new Mock<IClientFactory>();
        var loggerMock = new Mock<ILogger<TelnetServer>>();
        var settings = new ServerSettings { Host = "127.0.0.1", Port = 12345 };
        var options = Options.Create(settings);
        var randomProvider = new Mock<IRandomProvider>();

        var factory = new TelnetServerFactory(clientFactoryMock.Object, options, loggerMock.Object, randomProvider.Object);

        // Act
        var server = factory.CreateServer();

        // Assert
        Assert.NotNull(server);
        Assert.IsType<TelnetServer>(server);
    }

    [Fact]
    public void CreateServer_ReturnsDifferentInstances_ForDifferentCalls()
    {
        // Arrange
        var clientFactoryMock = new Mock<IClientFactory>();
        var loggerMock = new Mock<ILogger<TelnetServer>>();
        var optionsMock = new Mock<IOptions<ServerSettings>>();
        var settings1 = new ServerSettings { Host = "127.0.0.1", Port = 1000 };
        var settings2 = new ServerSettings { Host = "127.0.0.2", Port = 2000 };
        var randomProvider = new Mock<IRandomProvider>();

        var callCount = 0;
        optionsMock.Setup(o => o.Value).Returns(() =>
        {
            callCount++;
            return callCount == 1 ? settings1 : settings2;
        });

        var factory = new TelnetServerFactory(clientFactoryMock.Object, optionsMock.Object, loggerMock.Object, randomProvider.Object);

        // Act
        var server1 = factory.CreateServer();
        var server2 = factory.CreateServer();

        // Assert
        Assert.NotSame(server1, server2);
        Assert.IsType<TelnetServer>(server1);
        Assert.IsType<TelnetServer>(server2);
    }
}
