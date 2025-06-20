using JetBrains.Annotations;
using RebelAlliance.TelnetDemo.Telnet;

namespace RebelAlliance.TelnetDemo.UnitTests.Telnet;

[TestSubject(typeof(TelnetFilter))]
public class TelnetFilterTest
{
    [Fact]
    public void FilterCommands_ReturnsOriginalData_WhenNoCommandsPresent()
    {
        // Arrange
        var filter = new TelnetFilter();
        var input = new byte[] { 65, 66, 67 }; // 'A', 'B', 'C'

        // Act
        var result = filter.FilterCommands(input, input.Length);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void FilterCommands_RemovesTelnetCommandSequences()
    {
        // Arrange
        var filter = new TelnetFilter();
        var input = new byte[]
        {
            65, // 'A'
            255, 251, 1, // IAC WILL 1 (should be removed)
            66, // 'B'
            255, 252, 2, // IAC WONT 2 (should be removed)
            67 // 'C'
        };
        var expected = new byte[] { 65, 66, 67 };

        // Act
        var result = filter.FilterCommands(input, input.Length);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FilterCommands_HandlesEscapedIac()
    {
        // Arrange
        var filter = new TelnetFilter();
        var input = new byte[]
        {
            65, // 'A'
            255, 255, // IAC IAC (should become single IAC)
            66 // 'B'
        };
        var expected = new byte[] { 65, 66 };

        // Act
        var result = filter.FilterCommands(input, input.Length);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FilterCommands_SkipsSingleIacAtEnd()
    {
        // Arrange
        var filter = new TelnetFilter();
        var input = new byte[] { 65, 255 }; // 'A', IAC
        var expected = new byte[] { 65 };

        // Act
        var result = filter.FilterCommands(input, input.Length);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FilterCommands_SkipsIacWithUnknownOption()
    {
        // Arrange
        var filter = new TelnetFilter();
        var input = new byte[] { 65, 255, 200, 66 }; // 'A', IAC, 200 (unknown), 'B'
        var expected = new byte[] { 65, 66 };

        // Act
        var result = filter.FilterCommands(input, input.Length);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FilterCommands_HandlesEmptyInput()
    {
        // Arrange
        var filter = new TelnetFilter();
        var input = Array.Empty<byte>();

        // Act
        var result = filter.FilterCommands(input, 0);

        // Assert
        Assert.Empty(result);
    }
}