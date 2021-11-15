using System;
using Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{
    public Broadcaster<KeyCode> KeyCodeBroadcaster { get; } = new Broadcaster<KeyCode>();

    public InputManager()
    {
        Debug.Log("InputManager Created");
    }

    ~InputManager()
    {
        Debug.Log("InputManager OnDestroy");
    }

}
