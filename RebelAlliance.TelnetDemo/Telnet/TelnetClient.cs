using Microsoft.Extensions.Logging;
using RebelAlliance.TelnetDemo.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace RebelAlliance.TelnetDemo.Telnet;

public class TelnetClient(TcpClient tcpClient, 
    IChatRoom chatRoom, 
    short id, 
    ILogger<TelnetClient> logger,
    ITelnetFilter telnetFilter) : IClient, IDisposable
{
    private const string Eol = "\r\n";

    private string Name() => $"Client{id}";

    private string FormatMessage(string message)
    {
        return $"{Name()}: {message}\r\n";
    }

    public async Task Handle(CancellationToken cancellationToken)
    {
        var buffer = new byte[1024];
        var stream = tcpClient.GetStream();

        chatRoom.SendMessage($"{FormatMessage("joined the chat")}");

        var subscription = chatRoom.Subscribe(async void (message) =>
        {
            try
            {
                if (message.Contains(Name())) // skip this client messages
                    return;

                logger.LogTrace("[{Client}] Message received: {Message}", Name(), message);

                var response = Encoding.ASCII.GetBytes($"{message}{Eol}");
                if(!stream.CanWrite)
                {
                    logger.LogWarning("[{Client}] Stream is not writable, cannot send message", Name());
                    return;
                }

                await stream.WriteAsync(response, 0, response.Length, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send received message from broadcast");
            }
        });

        try
        {
            var response = Encoding.ASCII.GetBytes($"Welcome {Name()}!{Eol}");
            await stream.WriteAsync(response, 0, response.Length, cancellationToken);

            while (true)
            {
                var message = string.Empty;
                do
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead == 0)
                    {
                        throw new ClientDisconnectedException($"[{Name()}] Disconnected");
                    }

                    if (buffer[0] == 3)
                    {
                        throw new ClientWantsToLeaveException($"Client requested to exit.");
                    }

                    var cleanData = telnetFilter.FilterCommands(buffer, bytesRead);
                    message += Encoding.ASCII.GetString(cleanData);
                } while (!message.Contains(Eol));

                message = message.Replace(Eol, string.Empty);

                if (message.Length > 0)
                {
                    chatRoom.SendMessage($"{FormatMessage(message)}");
                    logger.LogTrace("[{Client}] Message Sent: {Message}", Name(), message);

                    var newLine = Encoding.ASCII.GetBytes(Eol);
                    await stream.WriteAsync(newLine, 0, newLine.Length, cancellationToken);
                }

                if(cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("[{Client}] Cancellation requested, exiting", Name());
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Client}] Error handling client connection", Name());
        }
        finally
        {
            chatRoom.SendMessage($"{FormatMessage("left the chat")}");
            tcpClient.Close();
            subscription.Dispose();
            Dispose();
        }
    }

    public void Dispose()
    {
        tcpClient.Dispose();
    }
}