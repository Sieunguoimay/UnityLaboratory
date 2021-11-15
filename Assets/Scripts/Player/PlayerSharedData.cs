using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class PlayerSharedData : Broadcaster<PlayerSharedData>
{
    public PlayerSharedData()
    {
        SetData(this);

        Debug.Log("PlayerSharedData Created");
    }

    ~PlayerSharedData()
    {
        Debug.Log("PlayerSharedData Destroyed");
    }

    public void Setup(PlayerSharedConfigData sharedConfigData)
    {
        Config = sharedConfigData;
    }

    public PlayerSharedConfigData Config { get; private set; }
    public PlayerSharedStateData SharedStateData { get; } = new PlayerSharedStateData();

    public class PlayerSharedStateData
    {
        public float Score = 0f;
        public float Speed = 0f;
    }
}