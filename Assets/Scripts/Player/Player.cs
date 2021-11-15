using System;
using Core;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour, IBroadcastListener<PlayerSharedData>
{
    private PlayerSharedData _sharedData;

    public void Setup(PlayerSharedData sharedData)
    {
        _sharedData = sharedData;
        _sharedData.Registrar.Register(nameof(Player), this, 1);
    }

    public void ModifyData()
    {
        _sharedData.SharedStateData.Score = 10;
        _sharedData.SharedStateData.Speed = 5;
        _sharedData.Broadcast();
    }

    private void OnDestroy()
    {
        Debug.Log("Player OnDestroyed");
    }

    public void OnReceiveBroadcastData(PlayerSharedData stateData)
    {
        Debug.Log(name + ":New Data is " + stateData.SharedStateData.Score + " " + stateData.SharedStateData.Speed);
    }
}