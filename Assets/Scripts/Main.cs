using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class Main : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject canvas;
    [SerializeField] private PlayerSharedConfigData playerSharedConfigDataAsset;

    private Player _player;

    private void Awake()
    {
        Debug.Log("Awake");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    void Start()
    {
        Debug.Log("Start");

        _player = Instantiate(playerPrefab, transform).GetComponent<Player>();
        var playerUI = Instantiate(playerUIPrefab, canvas.transform).GetComponent<PlayerUI>();
        var sharedData = new PlayerSharedData();
        Locator.Instance.PlayerSharedData = sharedData;

        sharedData.Setup(playerSharedConfigDataAsset);
        _player.Setup(sharedData);
        playerUI.Setup(sharedData);

        Locator.Instance.InvokeBroadcaster();
    }

    void Update()
    {
        // Debug.Log("Update");

        if (Input.GetKeyDown(KeyCode.X))
        {
            Destroy(_player.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _player.gameObject.SetActive(false);
            InputManager.Instance.KeyCodeBroadcaster.SetData(KeyCode.D);
            InputManager.Instance.KeyCodeBroadcaster.Broadcast(8);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            InputManager.Instance.KeyCodeBroadcaster.SetData(KeyCode.A);
            InputManager.Instance.KeyCodeBroadcaster.Broadcast(1);
            GC.Collect();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            InputManager.Instance.KeyCodeBroadcaster.SetData(KeyCode.B);
            InputManager.Instance.KeyCodeBroadcaster.Broadcast(2);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            InputManager.Instance.KeyCodeBroadcaster.SetData(KeyCode.C);
            InputManager.Instance.KeyCodeBroadcaster.Broadcast(4);
        }
    }

    void FixedUpdate()
    {
        // Debug.Log("FixedUpdate");
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
        InputManager.Instance.Cleanup();
        Locator.Instance.Cleanup();
    }
}