using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private readonly SyncVar<PlayerClass> _playerClass = new SyncVar<PlayerClass>();

    public PlayerClass PlayerClass
    {
        get => _playerClass.Value;
        set => _playerClass.Value = value;
    }

    private void Awake()
    {
        _playerClass.OnChange += OnPlayerClassChanged;
    }

    private void OnDestroy()
    {
        _playerClass.OnChange -= OnPlayerClassChanged;
    }

    private void OnPlayerClassChanged(PlayerClass previous, PlayerClass current, bool asServer)
    {
        // Handle the player class change on both the client and the server
        Debug.Log($"Player class changed from {previous} to {current} asServer: {asServer}");
    }
}