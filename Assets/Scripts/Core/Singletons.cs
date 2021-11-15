using UnityEngine;

namespace Core
{
    public class Singleton<T> where T : new()
    {
        private static T _instance;
        public static T Instance => _instance == null ? (_instance = new T()) : _instance;

        public virtual void Cleanup()
        {
            _instance = default;
        }
    }
    public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;
        public static T Instance => _instance ? _instance : (_instance = CreateInstance<T>());
    }

}