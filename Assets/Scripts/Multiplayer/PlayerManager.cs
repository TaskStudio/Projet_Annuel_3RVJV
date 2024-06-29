using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using System.Collections.Generic;
using FishNet;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public List<GameObject> meleeAttackers;
    public List<GameObject> rangedAttackers;
    public List<GameObject> supports;
    public List<GameObject> tanks;

    private Dictionary<NetworkConnection, GameObject> playerInstances = new Dictionary<NetworkConnection, GameObject>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnClientConnected;
    }

    private void OnClientConnected(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            AssignPlayerClass(conn);
        }
    }

    [Server]
    private void AssignPlayerClass(NetworkConnection conn)
    {
        PlayerClass assignedClass = GetRandomPlayerClass();
        GameObject playerInstance = GetAvailableEntity(assignedClass);

        if (playerInstance != null)
        {
            playerInstance.GetComponent<NetworkObject>().GiveOwnership(conn);
            playerInstances[conn] = playerInstance;

            // Set player class on the Player component
            Player playerComponent = playerInstance.GetComponent<Player>();
            playerComponent.PlayerClass = assignedClass;
        }
        else
        {
            Debug.LogWarning("No available player instance to assign.");
        }
    }

    private PlayerClass GetRandomPlayerClass()
    {
        int randomValue = Random.Range(0, 4);
        return (PlayerClass)randomValue;
    }

    private GameObject GetAvailableEntity(PlayerClass playerClass)
    {
        List<GameObject> list = null;

        switch (playerClass)
        {
            case PlayerClass.MeleeAttacker:
                list = meleeAttackers;
                break;
            case PlayerClass.RangedAttacker:
                list = rangedAttackers;
                break;
            case PlayerClass.Support:
                list = supports;
                break;
            case PlayerClass.Tank:
                list = tanks;
                break;
        }

        foreach (var entity in list)
        {
            if (!entity.GetComponent<NetworkObject>().IsOwner)
            {
                return entity;
            }
        }

        return null;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        InstanceFinder.ServerManager.OnRemoteConnectionState -= OnClientConnected;
    }
}
