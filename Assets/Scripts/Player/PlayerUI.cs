using System;
using Core;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour, IBroadcastListener<PlayerSharedData>, IBroadcastListener<KeyCode>
{
    private PlayerSharedData _playerSharedData;

    private Text _text;

    public void Setup(PlayerSharedData playerSharedData)
    {
        _playerSharedData = playerSharedData;
        _text = GetComponentInChildren<Text>();

        _playerSharedData.Registrar.Register(nameof(PlayerUI), this, 1);
        InputManager.Instance.KeyCodeBroadcaster.Registrar.Register(nameof(PlayerUI), this, 1 | 2 | 4);
    }

    public void OnReceiveBroadcastData(PlayerSharedData stateData)
    {
        Debug.Log(name + ":New Data is " + stateData.SharedStateData.Score + " " + stateData.SharedStateData.Speed);
        _text.text = $"Speed: {_playerSharedData.SharedStateData.Speed}/{_playerSharedData.Config.maxSpeed}";
    }

    public void OnReceiveBroadcastData(KeyCode stateData)
    {
        Debug.Log(name + "<PlayerUI>:On Key Pressed " + stateData);
    }
}