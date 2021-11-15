using UnityEngine;

namespace Core
{
    public class Locator : Singleton<Locator>
    {
        public Locator()
        {
            Debug.Log("Locator Created");
        }

        ~Locator()
        {
            Debug.Log("Locator Destroyed");
        }

        public override void Cleanup()
        {
            base.Cleanup();
            _playerSharedData = null;
        }

        private PlayerSharedData _playerSharedData;

        public PlayerSharedData PlayerSharedData
        {
            get => _playerSharedData;
            set => _playerSharedData = _playerSharedData ?? value;
        }

        public Broadcaster<int> Broadcaster { get; } = new Broadcaster<int>();

        public void InvokeBroadcaster()
        {
            Broadcaster.Broadcast();
        }
    }
}