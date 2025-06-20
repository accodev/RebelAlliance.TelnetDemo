namespace RebelAlliance.TelnetDemo.Interfaces;

public interface IChatRoom : IObservable<string>
{
    void SendMessage(string message);
}
