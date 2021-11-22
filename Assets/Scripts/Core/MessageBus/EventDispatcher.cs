using System.Collections.Generic;

namespace Core.Unity.MessageBus
{
    public class EventDispatcher
    {
        private Dictionary<string, List<IEventListener>> _listenersDict = new Dictionary<string, List<IEventListener>>();

        public void Publish(string eventName, object extra)
        {
            if (_listenersDict.TryGetValue(eventName, out var list))
            {
                foreach (var l in list)
                {
                    l.OnReceiveEvent(eventName, extra);
                }
            }
        }

        public void Subscribe(IEventListener listener, string eventName)
        {
            if (_listenersDict.TryGetValue(eventName, out var list))
            {
                list.Add(listener);
            }
            else
            {
                _listenersDict.Add(eventName, new List<IEventListener> {listener});
            }
        }

        public void Unsubscribe(IEventListener listener, string eventName)
        {
        }

        public void Unsubscribe(IEventListener listener)
        {
            foreach (var d in _listenersDict)
            {
                d.Value.Remove(listener);
            }
        }
    }

    public interface IEventListener
    {
        void OnReceiveEvent(string eventName, object extra);
    }
}