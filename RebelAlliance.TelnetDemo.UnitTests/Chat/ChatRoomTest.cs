using JetBrains.Annotations;
using RebelAlliance.TelnetDemo.Chat;

namespace RebelAlliance.TelnetDemo.UnitTests.Chat;

[TestSubject(typeof(ChatRoom))]
public class ChatRoomTest
{
    private class TestObserver : IObserver<string>
    {
        public List<string> Received { get; } = [];
        public bool Completed { get; private set; }
        public Exception? Error { get; private set; }

        public void OnCompleted() => Completed = true;
        public void OnError(Exception error) => Error = error;
        public void OnNext(string value) => Received.Add(value);
    }

    [Fact]
    public void Subscribe_AddsObserver_AndReturnsDisposable()
    {
        // Arrange
        var chatRoom = new ChatRoom();
        var observer = new TestObserver();

        // Act
        var disposable = chatRoom.Subscribe(observer);

        // Assert
        Assert.NotNull(disposable);
        // Send a message to verify observer is subscribed
        chatRoom.SendMessage("hello");
        Assert.Contains("hello", observer.Received);

        // Cleanup
        disposable.Dispose();
    }

    [Fact]
    public void Dispose_RemovesObserver_NoLongerReceivesMessages()
    {
        // Arrange
        var chatRoom = new ChatRoom();
        var observer = new TestObserver();
        var disposable = chatRoom.Subscribe(observer);

        // Act
        disposable.Dispose();
        chatRoom.SendMessage("should not be received");

        // Assert
        Assert.DoesNotContain("should not be received", observer.Received);
    }

    [Fact]
    public void SendMessage_DeliversToAllSubscribers()
    {
        // Arrange
        var chatRoom = new ChatRoom();
        var observer1 = new TestObserver();
        var observer2 = new TestObserver();
        var d1 = chatRoom.Subscribe(observer1);
        var d2 = chatRoom.Subscribe(observer2);

        // Act
        chatRoom.SendMessage("broadcast");

        // Assert
        Assert.Contains("broadcast", observer1.Received);
        Assert.Contains("broadcast", observer2.Received);

        // Cleanup
        d1.Dispose();
        d2.Dispose();
    }

    [Fact]
    public void MultipleDisposes_AreSafe()
    {
        // Arrange
        var chatRoom = new ChatRoom();
        var observer = new TestObserver();
        var disposable = chatRoom.Subscribe(observer);

        // Act
        disposable.Dispose();
        // Should not throw
        disposable.Dispose();
        chatRoom.SendMessage("not received");

        // Assert
        Assert.Empty(observer.Received);
    }

    [Fact]
    public void Subscribe_AllowsMultipleObservers()
    {
        // Arrange
        var chatRoom = new ChatRoom();
        var observers = Enumerable.Range(0, 5).Select(_ => new TestObserver()).ToList();
        var disposables = observers.Select(o => chatRoom.Subscribe(o)).ToList();

        // Act
        chatRoom.SendMessage("group");

        // Assert
        foreach (var observer in observers)
            Assert.Contains("group", observer.Received);

        // Cleanup
        foreach (var d in disposables)
            d.Dispose();
    }
}
