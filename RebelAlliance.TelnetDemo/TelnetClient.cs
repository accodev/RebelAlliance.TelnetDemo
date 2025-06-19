using System.Net.Sockets;
using System.Text;

namespace RebelAlliance.TelnetDemo;

internal class TelnetClient(TcpClient socket, ChatRoom chatRoom)
{
    private readonly int _id = Random.Shared.Next(0, 9999);

    public string Name() => $"Client{_id}";

    public string FormatMessage(string message)
    {
        return $"{Name()}: {message}";
    }

    public async Task Handle()
    {
        var buffer = new byte[1024];
        var stream = socket.GetStream();

        chatRoom.Subscribe(message =>
        {
            if (message.Contains(Name()))
                return;

            var response = Encoding.UTF8.GetBytes($"{message}");
            stream.Write(response, 0, response.Length);
        });

        try
        {
            while (true)
            {
                var msg = string.Empty;
                var label = Encoding.UTF8.GetBytes($"{FormatMessage(string.Empty)}");
                await stream.WriteAsync(label, 0, label.Length);

                while (!msg.Contains("\r\n"))
                {
                    var bytesReadInput = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesReadInput == 0)
                    {
                        // Client disconnected
                        Console.WriteLine("Client disconnected.");
                        break;
                    }

                    msg += Encoding.UTF8.GetString(buffer, 0, bytesReadInput);
                }

                if(msg.Length > 0)
                    chatRoom.SendMessage($"{FormatMessage(msg)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client connection error: {ex.Message}");
        }
        finally
        {
            socket.Close();
        }
    }
}