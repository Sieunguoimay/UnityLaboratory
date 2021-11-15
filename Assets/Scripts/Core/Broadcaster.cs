using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Broadcaster<T> : IBroadcastRegistrar<T>
    {
        public TupleDicRegistrar<IBroadcastListener<T>, int> Registrar { get; } = new TupleDicRegistrar<IBroadcastListener<T>, int>();

        private T _data;
        public void SetData(T data) => _data = data;

        public void Broadcast(int filter = ~0)
        {
            Registrar.RemoveItems();

            foreach (var l in Registrar)
            {
                if ((l.Value.Item2 & filter) != 0)
                {
                    l.Value.Item1.OnReceiveBroadcastData(_data);
                }
            }

            Registrar.RemoveItems();
        }
    }

    public interface IBroadcastListener<in T>
    {
        void OnReceiveBroadcastData(T stateData);
    }

    public interface IBroadcastRegistrar<T>
    {
        TupleDicRegistrar<IBroadcastListener<T>, int> Registrar { get; }
    }
}