using Microsoft.Extensions.Logging;
using RebelAlliance.TelnetDemo.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace RebelAlliance.TelnetDemo.Chat;

internal class ChatClient(TcpClient tcpClient, IChatRoom chatRoom, short id, ILogger<ChatClient> logger) : IClient
{
    public string Name() => $"Client{id}";

    public string FormatMessage(string message)
    {
        return $"{Name()}: {message}";
    }

    public async Task Handle(CancellationToken cancellationToken)
    {
        var buffer = new byte[1024];
        var stream = tcpClient.GetStream();

        chatRoom.Subscribe(message =>
        {
            if (message.Contains(Name())) // skip this client messages
                return;

            logger.LogTrace("[{Client}] Message received: {Message}", Name(), message);

            var response = Encoding.UTF8.GetBytes($"{message}");
            stream.Write(response, 0, response.Length);
        });

        try
        {
            while (true)
            {
                var label = Encoding.UTF8.GetBytes($"{FormatMessage(string.Empty)}");
                await stream.WriteAsync(label, cancellationToken);

                var msg = string.Empty;
                while (!msg.Contains("\r\n"))
                {
                    var bytesReadInput = await stream.ReadAsync(buffer, cancellationToken);
                    if (bytesReadInput == 0)
                    {
                        logger.LogInformation("[{Client}] Disconnected", Name());
                        break;
                    }

                    msg += Encoding.UTF8.GetString(buffer, 0, bytesReadInput);
                }

                if (msg.Length > 0)
                {
                    var message =
                        FormatMessage(msg);
                    chatRoom.SendMessage($"{message}");
                    logger.LogTrace("[{Client}] Message Sent: {Message}", Name(), message);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Client}] Error handling client connection", Name());
        }
        finally
        {
            tcpClient.Close();
        }
    }
}