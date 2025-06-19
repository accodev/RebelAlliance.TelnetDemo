namespace RebelAlliance.TelnetDemo.Interfaces;

internal interface IClient
{
    Task Handle(CancellationToken cancellationToken);
}
