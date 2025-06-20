namespace RebelAlliance.TelnetDemo.Interfaces;

internal interface ITelnetFilter
{
    byte[] FilterCommands(byte[] data, int length);
}
