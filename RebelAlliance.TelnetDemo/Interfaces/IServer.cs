namespace RebelAlliance.TelnetDemo.Interfaces;

internal interface IServer
{
    Task Execute(CancellationToken cancellationToken);
}
