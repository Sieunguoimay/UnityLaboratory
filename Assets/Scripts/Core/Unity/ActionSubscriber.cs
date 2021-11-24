using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Unity;
using UnityEngine;
using UnityEngine.Events;

public class ActionSubscriber : MonoBehaviour
{
    [SerializeField] public UnityEngine.Object source;
    [SerializeField] public ActionUnityEventPair[] actionUnityEventPairs;

    private void OnEnable()
    {
        Setup(true);
    }

    private void OnDisable()
    {
        Setup(false);
    }

    [Serializable]
    public class ActionUnityEventPair
    {
        public int sourceInstanceId;
        public string actionName;
        public UnityEvent unityEvent;
        public Delegate InvokeEventDelegate;

        public void InvokeEvent()
        {
            unityEvent?.Invoke();
        }
    }

    public void ExtractData()
    {
        if (source is GameObject go)
        {
            var components = go.GetComponents<Component>();
            foreach (var c in components)
            {
                ExtractType(c.GetType(), c.GetInstanceID());
            }
        }
        else if (source is Component c)
        {
            ExtractType(c.GetType(), c.GetInstanceID());
        }
        else
        {
            ExtractType(source.GetType(), source.GetInstanceID());
        }
    }

    private void ExtractType(Type type, int id)
    {
        actionUnityEventPairs = new ActionUnityEventPair[0];
        var events = type.GetEvents(); //.Where(t => t.FieldType == typeof(Action));
        foreach (var e in events)
        {
            CreatePair(e, id);
        }
    }

    private void CreatePair(EventInfo eventInfo, int id)
    {
        if (actionUnityEventPairs == null) actionUnityEventPairs = new ActionUnityEventPair[0];
        ArrayUtils.Add(ref actionUnityEventPairs, new ActionUnityEventPair()
        {
            sourceInstanceId = id,
            actionName = eventInfo.Name,
            unityEvent = new UnityEvent()
        });
    }

    private void Setup(bool sub)
    {
        if (source is GameObject go)
        {
            var components = go.GetComponents<Component>();
            foreach (var c in components)
            {
                if (sub)
                {
                    Subscribe(c);
                }
                else
                {
                    Unsubscribe(c);
                }
            }
        }
        else if (source is Component c)
        {
            if (sub)
            {
                Subscribe(c);
            }
            else
            {
                Unsubscribe(c);
            }
        }
        else
        {
            if (sub)
            {
                Subscribe(source);
            }
            else
            {
                Unsubscribe(source);
            }
        }
    }

    private void Subscribe(UnityEngine.Object obj)
    {
        foreach (var p in actionUnityEventPairs)
        {
            if (obj.GetInstanceID() == p.sourceInstanceId)
            {
                var eventInfo = obj.GetType().GetEvent(p.actionName);
                var d = eventInfo.EventHandlerType;
                var methodInfoHandler = p.GetType().GetMethod("InvokeEvent", BindingFlags.Public |BindingFlags.NonPublic | BindingFlags.Instance);
                if (methodInfoHandler != null)
                {
                    var newDelegate = Delegate.CreateDelegate(d, p, methodInfoHandler);
                    var addHandler = eventInfo.GetAddMethod();
                    object[] addHandlerArgs = {newDelegate};
                    addHandler.Invoke(obj, addHandlerArgs);
                    p.InvokeEventDelegate = newDelegate;
                }
            }
        }
    }

    private void Unsubscribe(UnityEngine.Object obj)
    {
        foreach (var p in actionUnityEventPairs)
        {
            if (obj.GetInstanceID() == p.sourceInstanceId && p.InvokeEventDelegate != null)
            {
                var eventInfo = obj.GetType().GetEvent(p.actionName);
                var removeHandler = eventInfo.GetRemoveMethod();
                object[] removeHandlerArgs = {p.InvokeEventDelegate};
                removeHandler.Invoke(obj, removeHandlerArgs);
            }
        }
    }
}