using System;
using Core;
using UnityEngine;

public class PlayerController : MonoBehaviour, IBroadcastListener<KeyCode>
{
    private Player _player;

    private void Start()
    {
        _player = GetComponent<Player>();
        InputManager.Instance.KeyCodeBroadcaster.Registrar.Register(nameof(PlayerController), this, 4);
    }

    private void OnDestroy()
    {
        InputManager.Instance.KeyCodeBroadcaster.Registrar.Unregister(nameof(PlayerController));
    }

    public void OnReceiveBroadcastData(KeyCode stateData)
    {
        Debug.Log(name + "<PlayerController>:On Key Pressed " + stateData);
        switch (stateData)
        {
            case KeyCode.A:
                break;
            case KeyCode.C:
                _player.ModifyData();
                break;
        }
    }
}