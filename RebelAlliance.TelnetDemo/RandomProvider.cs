using RebelAlliance.TelnetDemo.Interfaces;

namespace RebelAlliance.TelnetDemo;

public class RandomProvider : IRandomProvider
{
    public short Provide(short min, short max) => (short)Random.Shared.Next(min, max);
}
