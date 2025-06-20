namespace RebelAlliance.TelnetDemo.Interfaces;

public interface IClient
{
    Task Handle(CancellationToken cancellationToken);
}
