using Microsoft.Extensions.Logging;
using RebelAlliance.TelnetDemo.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace RebelAlliance.TelnetDemo.Telnet;

internal class TelnetClient(TcpClient tcpClient, IChatRoom chatRoom, short id, ILogger<TelnetClient> logger) : IClient
{
    private const string EOL = "\r\n";

    private string Name() => $"Client{id}";

    private static byte[] FilterTelnetCommands(byte[] data, int length)
    {
        List<byte> result = [];
        for (int i = 0; i < length; i++)
        {
            if (data[i] == 255) // IAC
            {
                // Skip command and its option
                if (i + 1 < length && data[i + 1] >= 251 && data[i + 1] <= 254) i += 2;
                else if (i + 1 < length && data[i + 1] == 255) // Escaped 255
                {
                    result.Add(255);
                    i++;
                }
                else i++;
            }
            else
            {
                result.Add(data[i]);
            }
        }
        return [.. result];
    }

    private string FormatMessage(string message)
    {
        return $"{Name()}: {message}\r\n";
    }

    public async Task Handle(CancellationToken cancellationToken)
    {
        var buffer = new byte[1024];
        var stream = tcpClient.GetStream();

        chatRoom.SendMessage($"{FormatMessage("joined the chat")}");

        chatRoom.Subscribe(message =>
        {
            if (message.Contains(Name())) // skip this client messages
                return;

            logger.LogTrace("[{Client}] Message received: {Message}", Name(), message);

            var response = Encoding.ASCII.GetBytes($"{message}{EOL}");
            stream.Write(response, 0, response.Length);
        });

        try
        {
            var response = Encoding.ASCII.GetBytes($"Welcome!{EOL}");
            stream.Write(response, 0, response.Length);

            while (true)
            {
                var message = string.Empty;
                do
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        throw new Exception($"[{Name()}] Disconnected");
                    }

                    if (buffer[0] == 3)
                    {
                        throw new Exception($"Client requested to exit.");
                    }

                    var cleanData = FilterTelnetCommands(buffer, bytesRead);
                    message += Encoding.ASCII.GetString(cleanData);
                } while (!message.Contains(EOL));

                message = message.Replace(EOL, string.Empty);

                if (message.Length > 0)
                {
                    chatRoom.SendMessage($"{FormatMessage(message)}");
                    logger.LogTrace("[{Client}] Message Sent: {Message}", Name(), message);

                    var newLine = Encoding.ASCII.GetBytes(EOL);
                    stream.Write(newLine, 0, newLine.Length);
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
        }
    }
}