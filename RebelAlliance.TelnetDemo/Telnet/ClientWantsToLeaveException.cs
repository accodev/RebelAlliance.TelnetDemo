namespace RebelAlliance.TelnetDemo.Telnet;

internal class ClientWantsToLeaveException(string message) : Exception(message)
{
}