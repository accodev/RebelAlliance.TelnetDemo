namespace RebelAlliance.TelnetDemo;

internal class ChatRoom : IObservable<string>
{
    private readonly object _sync = new();
    private readonly List<User> _subscribers = [];

    public IDisposable Subscribe(IObserver<string> observer)
    {
        var sub = new User(this, observer);
        lock (this._sync)
        {
            this._subscribers.Add(sub);
        }

        return sub;
    }

    private void Unsubscribe(User sub)
    {
        lock (this._sync)
        {
            this._subscribers.Remove(sub);
        }
    }

    public void SendMessage(string message)
    {
        lock (this._sync)
        {
            foreach (var subscription in this._subscribers)
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
            this._parent?.Unsubscribe(this);
            this._parent = null;
        }
    }
}