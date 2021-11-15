using System.Collections.Generic;

namespace Core
{
    public class ListRegistrar<T> : List<T>
    {
        private readonly List<T> _itemsToRemove = new List<T>();

        public void Register(T item)
        {
            Add(item);
        }

        public void Unregister(T item)
        {
            _itemsToRemove.Add(item);
        }

        public void RemoveItems()
        {
            if (_itemsToRemove.Count <= 0) return;
            foreach (var i in _itemsToRemove)
            {
                Remove(i);
            }

            _itemsToRemove.Clear();
        }
    }

    public class DicRegistrar<T> : Dictionary<string, T>
    {
        private readonly List<string> _itemsToRemove = new List<string>();

        public void Register(string key, T item)
        {
            Add(key, item);
        }

        public void Unregister(string item)
        {
            _itemsToRemove.Add(item);
        }

        public void RemoveItems()
        {
            if (_itemsToRemove.Count <= 0) return;
            foreach (var i in _itemsToRemove)
            {
                Remove(i);
            }

            _itemsToRemove.Clear();
        }
    }

    public class TupleDicRegistrar<T, TT> : DicRegistrar<(T, TT)>
    {
        public void Register(string key, T item, TT extraData)
        {
            Register(key, (item, extraData));
        }
    }
}