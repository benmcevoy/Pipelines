I have started looking at some of the event aggregation and messaging patterns around MVVM.  [PRISM](https://compositewpf.codeplex.com/) offers an implementation and I have dissected it down to a simplistic implementation in order to learn a bit more.

This is EXPERIMENTAL - the actual implementation has WeakReferences, thread safety etc which are not present here.  However, this is simple and it works (to a degree) and, I think, captures the essence of the PRISM event aggregation code.

## The Aggregator ##

<pre><code>

    public class Aggregator
    {
        private readonly Dictionary&lt;Type, EventBase&gt; _events;

        public Aggregator()
        {
            _events = new Dictionary&lt;Type, EventBase&gt;();
        }

        public TEventType Get&lt;TEventType&gt;() where TEventType : EventBase, new()
        {
            EventBase newEvent = null;

            if (!_events.TryGetValue(typeof(TEventType), out newEvent))
            {
                newEvent = new TEventType();
                _events[typeof(TEventType)] = newEvent;
            }

            return (TEventType)newEvent;
        }
    }

</code></pre>

## The Event ##

<pre><code>

    public abstract class EventBase
    {
        private readonly List&lt;Action&lt;object&gt;&gt; _subscriptions;

        public EventBase()
        {
            _subscriptions = new List&lt;Action&lt;object&gt;&gt;();
        }

        protected void PublishImpl(object payload)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription(payload);
            }
        }

        protected void SubscribeImpl(Action&lt;object&gt; subscription)
        {
            _subscriptions.Add(subscription);
        }
    }

</code></pre>

## An Event and Usage ##

<pre><code>

    public class NotificationEvent : EventBase
    {
        public void Publish(string message)
        {
            base.PublishImpl(message);
        }

        public void Subscribe(Action&lt;string&gt; action)
        {
            var subscription = new Action&lt;object&gt;((payload) =&gt; action((string)payload));
            base.SubscribeImpl(subscription);
        }
    }

    ...

    _aggregator.Get&lt;NotificationEvent&gt;().Publish("sending a message...");

    _aggregator.Get&lt;NotificationEvent&gt;().Subscribe((message) =&gt; { DoSomethingWithTheMessage(message); });

    ...

</code></pre>


That's it.  My intention is to dissect the GalaSoft MMVM Light Messenger implementation and do a bit of a compare and contrast.  Let's see how I go.
