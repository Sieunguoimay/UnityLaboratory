using System;
using System.Linq;
using Core.Unity.MessageBus;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unity
{
    public class EventListener : MonoBehaviour, IEventListener
    {
        [SerializeField] public EventGroup[] eventGroups = new EventGroup[0];
        private const string DefaultEventName = "Event Name";

        private void OnEnable()
        {
            foreach (var eg in eventGroups)
            {
                foreach (var en in eg.eventNames)
                {
                    Locator.Instance.EventDispatcher.Subscribe(this, en);
                }
            }
        }

        private void OnDisable()
        {
            Locator.Instance.EventDispatcher.Unsubscribe(this);
        }

        public void OnReceiveEvent(string eventName, object extra)
        {
            foreach (var eg in eventGroups)
            {
                for (int i = 0; i < eg.eventNames.Length; i++)
                {
                    if (eg.eventNames[i].Equals(eventName))
                    {
                        eg.invoker?.Invoke();
                    }
                }
            }
        }

        public void AddEventGroup()
        {
            ArrayUtils.Add(ref eventGroups, new EventGroup()
            {
                eventNames = new[] {DefaultEventName},
                invoker = new UnityEvent()
            });
        }

        public void RemoveEventGroup()
        {
            if (eventGroups.Length > 0)
                ArrayUtils.Remove(ref eventGroups, eventGroups.Length - 1);
        }

        public void AddEventName(int eventGroupIndex)
        {
            if (eventGroupIndex < eventGroups.Length)
            {
                ArrayUtils.Add(ref eventGroups[eventGroupIndex].eventNames, DefaultEventName);
            }
        }

        public void RemoveEventName(int eventGroupIndex)
        {
            if (eventGroups[eventGroupIndex].eventNames.Length > 0)
            {
                ArrayUtils.Remove(ref eventGroups[eventGroupIndex].eventNames, eventGroups[eventGroupIndex].eventNames.Length - 1);
            }
        }

        [Serializable]
        public class EventGroup
        {
            [SerializeField] public string[] eventNames;
            [SerializeField] public UnityEvent invoker;
        }
    }
}