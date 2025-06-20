namespace RebelAlliance.TelnetDemo.Interfaces;

public interface ITelnetFilter
{
    byte[] FilterCommands(byte[] data, int length);
}
