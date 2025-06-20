namespace RebelAlliance.TelnetDemo.Interfaces;

public interface IServer
{
    Task Execute(CancellationToken cancellationToken);
}
