using UnityEngine;

[CreateAssetMenu(fileName = nameof(PlayerSharedConfigData), menuName = nameof(PlayerSharedConfigData))]
public class PlayerSharedConfigData : ScriptableObject
{
    public float maxSpeed;
}