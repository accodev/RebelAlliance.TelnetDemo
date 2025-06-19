using RebelAlliance.TelnetDemo.Interfaces;

namespace RebelAlliance.TelnetDemo.Chat;

internal class ChatRoom : IChatRoom
{
    private readonly object _sync = new();
    private readonly List<User> _subscribers = [];

    public IDisposable Subscribe(IObserver<string> observer)
    {
        var sub = new User(this, observer);
        lock (_sync)
        {
            _subscribers.Add(sub);
        }

        return sub;
    }

    private void Unsubscribe(User sub)
    {
        lock (_sync)
        {
            _subscribers.Remove(sub);
        }
    }

    public void SendMessage(string message)
    {
        lock (_sync)
        {
            foreach (var subscription in _subscribers)
            {
                subscription.Observer.OnNext(message);
            }
        }
    }

    private class User(ChatRoom parent, IObserver<string> observer) : IDisposable
    {
        private ChatRoom? _parent = parent;

        public IObserver<string> Observer { get; } = observer;

        public void Dispose()
        {
            _parent?.Unsubscribe(this);
            _parent = null;
        }
    }
}