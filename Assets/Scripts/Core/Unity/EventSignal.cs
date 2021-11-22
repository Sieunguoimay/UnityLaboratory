using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unity
{
    public class EventSignal : MonoBehaviour
    {
        [SerializeField] private EventGroup[] eventGroups;
        [SerializeField] private string[] filterGroups;

        [Serializable]
        public class EventGroup
        {
            public int filter;
            public UnityEvent onSignal = new UnityEvent();
        }

        public void Signal(string filterName)
        {
            var filterIndex = -1;
            for (var i = 0; i < filterGroups.Length; i++)
            {
                if (filterGroups[i].Equals(filterName))
                {
                    filterIndex = i;
                    break;
                }
            }

            if (filterIndex < 0) return;

            var filter = Mathf.CeilToInt(Mathf.Pow(2, filterIndex));

            Signal(filter);
        }

        public void Signal(int filter)
        {
            for (int i = 0; i < eventGroups.Length; i++)
            {
                if ((eventGroups[i].filter & filter) != 0)
                {
                    eventGroups[i].onSignal?.Invoke();
                }
            }
        }

        public bool CheckFilterByIndex(int index, int filter)
        {
            var f = Mathf.CeilToInt(Mathf.Pow(2, index));
            return (f & filter) != 0;
        }

        [ContextMenu("Signal All")]
        public void SignalAll()
        {
            Signal(~0);
        }

        public void AddNewEventGroup()
        {
            ArrayUtils.Add(ref eventGroups, new EventGroup());
        }

        public void RemoveEventGroup()
        {
            if (eventGroups.Length > 0)
                ArrayUtils.Remove(ref eventGroups, eventGroups.Length - 1);
        }

        public void AddFilterGroup()
        {
            ArrayUtils.Add(ref filterGroups, "");
        }

        public void RemoveFilterGroup()
        {
            if (filterGroups.Length > 0)
                ArrayUtils.Remove(ref filterGroups, filterGroups.Length - 1);
        }

        public void RemoveFilterGroup(int index)
        {
            var pow = Mathf.CeilToInt(Mathf.Pow(2, filterGroups.Length - 1));
            foreach (var eg in eventGroups)
            {
                for (int i = index; i < filterGroups.Length - 1; i++)
                {
                    var pow2 = Mathf.CeilToInt(Mathf.Pow(2, i + 1));
                    var pow3 = Mathf.CeilToInt(Mathf.Pow(2, i));
                    if ((eg.filter & pow2) != 0)
                    {
                        eg.filter |= pow3;
                    }

                    else
                    {
                        eg.filter &= ~pow3;
                    }
                }

                eg.filter &= ~pow;
            }

            if (index < filterGroups.Length)
                ArrayUtils.Remove(ref filterGroups, index);
        }
    }
}