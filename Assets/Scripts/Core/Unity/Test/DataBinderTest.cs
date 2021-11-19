using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Core.Unity
{
    public class DataBinderTest : MonoBehaviour, IBroadcastListener<int>
    {
        [SerializeField] private string broadcasterName;
        [SerializeField] private int filter;

        private object _dataValue;
        private Type _dataType;

        private PropertyInfo _broadcasterPropInfo;
        private object _broadcasterValue;
        private Type _broadcasterType;

        private object _broadcastListenerValue;
        private Type _broadcastListenerType;

        private PropertyInfo _registrarPropInfo;
        private object _registrarValue;
        private Type _registrarPropType;
        private MethodInfo _unregisterMethInfo;
        private MethodInfo _registerMethInfo;

        private bool _initialized;
        private bool _subscribed;

        private void Awake()
        {
            Locator.Instance.Broadcaster.Registrar.Register(nameof(DataBinderTest), this, ~0);
            SetupType();
        }

        private void OnEnable()
        {
            if (!_subscribed && _initialized)
            {
                Subscribe();
                _subscribed = true;
            }
        }

        private void OnDisable()
        {
            if (_subscribed)
            {
                Unsubscribe();
                _subscribed = false;
            }
        }

        private void OnDestroy()
        {
            Locator.Instance.Broadcaster.Registrar.Unregister(nameof(DataBinderTest));
        }

        public void OnReceiveBroadcastData(int stateData)
        {
            SetupValue();
            if (!_subscribed && enabled)
            {
                Subscribe();
                _subscribed = true;
            }
        }

        private void SetupType()
        {
            var locatorType = typeof(Locator);

            _broadcasterPropInfo = locatorType.GetProperty(broadcasterName);

            if (_broadcasterPropInfo == null)
            {
                Debug.LogError($"Failed to GetProperty {broadcasterName} from Locator");
                return;
            }

            _broadcasterType = _broadcasterPropInfo.PropertyType;

            _registrarPropInfo = _broadcasterType.GetProperty("Registrar");

            if (_registrarPropInfo == null)
            {
                Debug.LogError("Failed to GetProperty <Registrar>");
                return;
            }

            _registrarPropType = _registrarPropInfo.PropertyType;

            _dataType = _broadcasterType.BaseType?.GenericTypeArguments[0];

            Type[] typeArgs = {_dataType};

            var broadcastListenerEmptyType = typeof(BroadcastListener<>);

            _broadcastListenerType = broadcastListenerEmptyType.MakeGenericType(typeArgs);

            _registerMethInfo = _registrarPropType.GetMethod("Register", new[] {typeof(string), _broadcastListenerType, typeof(int)});

            _unregisterMethInfo = _registrarPropType.GetMethod("Unregister", new[] {typeof(string)});

            if (_registerMethInfo == null)
            {
                Debug.LogError("Failed to GetMethod <Register>");
                return;
            }

            if (_unregisterMethInfo == null)
            {
                Debug.LogError("Failed to GetMethod <Unregister>");
                return;
            }
        }

        private void SetupValue()
        {
            var locator = Locator.Instance;

            _broadcasterValue = _broadcasterPropInfo.GetValue(locator);

            _registrarValue = _registrarPropInfo.GetValue(_broadcasterValue, null);

            _broadcastListenerValue = Activator.CreateInstance(_broadcastListenerType, new object[] {this});

            if (_broadcasterValue == null)
            {
                Debug.LogError($"Failed to Get Data {broadcasterName} from Locator");
                return;
            }

            _initialized = true;
        }

        [ContextMenu("Subscribe")]
        private void Subscribe()
        {
            if (!_initialized) return;

            _registerMethInfo.Invoke(_registrarValue, new[] {_broadcastListenerType.Name, _broadcastListenerValue, filter});

            Debug.Log($"DataBinder {gameObject.name} has subscribed to {broadcasterName}");
        }

        [ContextMenu("Unsubscribe")]
        private void Unsubscribe()
        {
            if (!_initialized) return;

            _unregisterMethInfo?.Invoke(_registrarValue, new object[] {_broadcastListenerType.Name});

            Debug.Log($"DataBinder {gameObject.name} has unsubscribed from {broadcasterName}");
        }

        [ContextMenu("Print datatype info")]
        private void PrintDataTypeInfo()
        {
            SetupType();

            var propsInfo = _dataType.GetProperties();
            foreach (var p in propsInfo)
            {
                var pt = p.PropertyType;

                Debug.Log(p.Name);

                if (pt.IsClass)
                {
                    var pfsInfo = pt.GetFields();
                    foreach (var pf in pfsInfo)
                    {
                        Debug.Log(pf.Name);
                    }
                }
            }
        }

        private void UpdateTarget(object obj)
        {
            _dataValue = obj;

            var propsInfo = _dataType.GetProperties();
            foreach (var p in propsInfo)
            {
                var pt = p.PropertyType;
                var pv = p.GetValue(_dataValue, null);
                if (pt.IsClass)
                {
                    var pfsInfo = pt.GetFields();
                    foreach (var pf in pfsInfo)
                    {
                        var value = pf.GetValue(pv);
                        Debug.Log(pf.Name + ": " + value);
                    }
                }
            }
        }

        public void SetInt(int d, int e, int f)
        {
        }

        public void SetInt(int d, int e)
        {
        }

        public void SetInt(int d)
        {
        }

        private bool a;
        public void SetString(string s)
        {
            Debug.Log("Set to " + s);
        }

        public class BroadcastListener<T> : IBroadcastListener<T>
        {
            private readonly DataBinderTest _binderTest;

            public BroadcastListener(DataBinderTest binderTest)
            {
                _binderTest = binderTest;
            }

            public void OnReceiveBroadcastData(T stateData)
            {
                _binderTest.UpdateTarget(stateData);
            }
        }

        [SerializeField] public List<ColorPoint> colorPoints;

        [Serializable]
        public class ColorPoint
        {
            public Vector3 position;
            public Color color;
        }
    }
}