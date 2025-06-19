namespace RebelAlliance.TelnetDemo.Interfaces;

internal interface IChatRoom : IObservable<string>
{
    void SendMessage(string message);
}
