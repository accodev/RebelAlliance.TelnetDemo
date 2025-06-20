using RebelAlliance.TelnetDemo.Interfaces;

namespace RebelAlliance.TelnetDemo.Telnet;

public class TelnetFilter : ITelnetFilter
{
    private enum Command
    {
        Will = 251,
        Wont = 252,
        Do = 253,
        Dont = 254,
        Iac = 255
    }

    public byte[] FilterCommands(byte[] data, int length)
    {
        List<byte> result = [];
        for (var i = 0; i < length; i++)
        {
            if (data[i] == (byte)Command.Iac)
            {
                // Skip command and its option
                if (i + 1 < length && data[i + 1] >= (byte)Command.Will && data[i + 1] <= (byte)Command.Dont) i += 2;
                else if (i + 1 < length && data[i + 1] == (byte)Command.Iac) // Escaped IAC
                {
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
}